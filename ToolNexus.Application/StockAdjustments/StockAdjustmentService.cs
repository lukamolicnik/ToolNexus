using ToolNexus.Application.StockAdjustments.DTOs;
using ToolNexus.Domain.StockAdjustments;

namespace ToolNexus.Application.StockAdjustments;

public class StockAdjustmentService : IStockAdjustmentService
{
    private readonly IStockAdjustmentRepository _stockAdjustmentRepository;

    public StockAdjustmentService(IStockAdjustmentRepository stockAdjustmentRepository)
    {
        _stockAdjustmentRepository = stockAdjustmentRepository;
    }

    public async Task<List<StockAdjustmentDto>> GetAllAsync()
    {
        var adjustments = await _stockAdjustmentRepository.GetAllAsync();
        return adjustments.Select(MapToDto).ToList();
    }

    public async Task<List<StockAdjustmentDto>> GetByToolIdAsync(int toolId)
    {
        var adjustments = await _stockAdjustmentRepository.GetByToolIdAsync(toolId);
        return adjustments.Select(MapToDto).ToList();
    }

    public async Task<StockAdjustmentDto?> GetByIdAsync(int id)
    {
        var adjustment = await _stockAdjustmentRepository.GetByIdAsync(id);
        return adjustment != null ? MapToDto(adjustment) : null;
    }

    public async Task<List<StockAdjustmentDto>> GetRecentAdjustmentsAsync(int count = 50)
    {
        var adjustments = await _stockAdjustmentRepository.GetRecentAdjustmentsAsync(count);
        return adjustments.Select(MapToDto).ToList();
    }

    private static StockAdjustmentDto MapToDto(StockAdjustment adjustment)
    {
        return new StockAdjustmentDto
        {
            Id = adjustment.Id,
            ToolId = adjustment.ToolId,
            ToolCode = adjustment.Tool?.Code ?? "",
            ToolName = adjustment.Tool?.Name ?? "",
            AdjustmentType = adjustment.AdjustmentType,
            Quantity = adjustment.Quantity,
            PreviousStock = adjustment.PreviousStock,
            NewStock = adjustment.NewStock,
            Reason = adjustment.Reason,
            Notes = adjustment.Notes,
            AdjustedBy = adjustment.AdjustedBy,
            AdjustedAt = adjustment.AdjustedAt
        };
    }
}