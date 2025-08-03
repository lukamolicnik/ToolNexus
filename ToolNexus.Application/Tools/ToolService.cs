using ToolNexus.Application.Tools.DTOs;
using ToolNexus.Domain.Tools;
using ToolNexus.Domain.StockAdjustments;

namespace ToolNexus.Application.Tools
{
    public class ToolService : IToolService
    {
        private readonly IToolRepository _toolRepository;
        private readonly IStockAdjustmentRepository _stockAdjustmentRepository;

        public ToolService(IToolRepository toolRepository, IStockAdjustmentRepository stockAdjustmentRepository)
        {
            _toolRepository = toolRepository;
            _stockAdjustmentRepository = stockAdjustmentRepository;
        }

        public async Task<List<ToolDto>> GetAllToolsAsync()
        {
            var tools = await _toolRepository.GetAllToolsAsync();
            return tools.Select(MapToDto).ToList();
        }

        public async Task<ToolDto> GetToolByIdAsync(int id)
        {
            var tool = await _toolRepository.GetByIdAsync(id);
            if (tool == null)
                throw new Exception($"Orodje z ID {id} ne obstaja.");
            return MapToDto(tool);
        }

        public async Task<ToolDto> CreateToolAsync(CreateToolDto toolDto, string userId)
        {
            // Check if tool with same code already exists
            var existingTool = await _toolRepository.GetByCodeAsync(toolDto.Code);
            if (existingTool != null)
                throw new Exception($"Orodje s kodo '{toolDto.Code}' že obstaja.");

            var tool = new Tool
            {
                Code = toolDto.Code,
                Name = toolDto.Name,
                Description = toolDto.Description,
                ToolCategoryId = toolDto.ToolCategoryId,
                MinimumStock = toolDto.MinimumStock,
                CriticalStock = toolDto.CriticalStock,
                CurrentStock = 0,
                CreatedBy = userId,
                UpdatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _toolRepository.AddAsync(tool);
            return MapToDto(tool);
        }

        public async Task<ToolDto> UpdateToolAsync(int id, UpdateToolDto toolDto, string userId)
        {
            var tool = await _toolRepository.GetByIdAsync(id);
            if (tool == null)
                throw new Exception($"Orodje z ID {id} ne obstaja.");

            tool.Name = toolDto.Name;
            tool.Description = toolDto.Description;
            tool.MinimumStock = toolDto.MinimumStock;
            tool.CriticalStock = toolDto.CriticalStock;
            tool.ToolCategoryId = toolDto.ToolCategoryId;
            tool.UpdatedBy = userId;

            await _toolRepository.UpdateAsync(tool);
            return MapToDto(tool);
        }

        public async Task DeleteToolAsync(int id)
        {
            var tool = await _toolRepository.GetByIdAsync(id);
            if (tool == null)
                throw new Exception($"Orodje z ID {id} ne obstaja.");
                
            if (tool.CurrentStock > 0)
                throw new Exception($"Orodja ni mogoče izbrisati, ker ima zalogo ({tool.CurrentStock} kos).");
                
            await _toolRepository.DeleteAsync(tool);
        }

        public async Task<ToolDto> IncreaseStockAsync(IncreaseStockDto increaseStockDto)
        {
            var tool = await _toolRepository.GetByIdAsync(increaseStockDto.ToolId);
            if (tool == null)
                throw new Exception($"Orodje z ID {increaseStockDto.ToolId} ne obstaja.");

            var previousStock = tool.CurrentStock;
            tool.CurrentStock += increaseStockDto.Quantity;
            tool.UpdatedBy = increaseStockDto.UserId;
            tool.UpdatedAt = DateTime.UtcNow;

            await _toolRepository.UpdateAsync(tool);
            
            // Log stock adjustment
            var adjustment = new StockAdjustment
            {
                ToolId = tool.Id,
                AdjustmentType = StockAdjustmentType.Increase,
                Quantity = increaseStockDto.Quantity,
                PreviousStock = previousStock,
                NewStock = tool.CurrentStock,
                Notes = increaseStockDto.Notes,
                AdjustedBy = increaseStockDto.UserId,
                AdjustedAt = DateTime.UtcNow
            };
            
            await _stockAdjustmentRepository.AddAsync(adjustment);
            
            return MapToDto(tool);
        }

        public async Task<ToolDto> DecreaseStockAsync(DecreaseStockDto decreaseStockDto)
        {
            var tool = await _toolRepository.GetByIdAsync(decreaseStockDto.ToolId);
            if (tool == null)
                throw new Exception($"Orodje z ID {decreaseStockDto.ToolId} ne obstaja.");

            if (tool.CurrentStock < decreaseStockDto.Quantity)
                throw new InvalidOperationException($"Ni dovolj zaloge. Trenutna zaloga: {tool.CurrentStock}, zahtevana količina: {decreaseStockDto.Quantity}");

            var previousStock = tool.CurrentStock;
            tool.CurrentStock -= decreaseStockDto.Quantity;
            tool.UpdatedBy = decreaseStockDto.UserId;
            tool.UpdatedAt = DateTime.UtcNow;

            await _toolRepository.UpdateAsync(tool);
            
            // Log stock adjustment with reason
            var adjustment = new StockAdjustment
            {
                ToolId = tool.Id,
                AdjustmentType = StockAdjustmentType.Decrease,
                Quantity = decreaseStockDto.Quantity,
                PreviousStock = previousStock,
                NewStock = tool.CurrentStock,
                Reason = decreaseStockDto.Reason,
                Notes = decreaseStockDto.Notes,
                AdjustedBy = decreaseStockDto.UserId,
                AdjustedAt = DateTime.UtcNow
            };
            
            await _stockAdjustmentRepository.AddAsync(adjustment);
            
            return MapToDto(tool);
        }

        private static ToolDto MapToDto(Tool tool)
        {
            return new ToolDto
            {
                Id = tool.Id,
                Code = tool.Code,
                Name = tool.Name,
                Description = tool.Description,
                ToolCategoryId = tool.ToolCategoryId,
                ToolCategoryName = tool.ToolCategory?.Name,
                CurrentStock = tool.CurrentStock,
                MinimumStock = tool.MinimumStock,
                CriticalStock = tool.CriticalStock,
                IsBelowMinimum = tool.IsBelowMinimum,
                IsCritical = tool.IsCritical,
                CreatedAt = tool.CreatedAt,
                CreatedBy = tool.CreatedBy,
                UpdatedAt = tool.UpdatedAt,
                UpdatedBy = tool.UpdatedBy
            };
        }
    }
}
