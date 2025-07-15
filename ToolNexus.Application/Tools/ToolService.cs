using ToolNexus.Application.Tools.DTOs;
using ToolNexus.Domain.Tools;

namespace ToolNexus.Application.Tools
{
    public class ToolService : IToolService
    {
        private readonly IToolRepository _toolRepository;

        public ToolService(IToolRepository toolRepository)
        {
            _toolRepository = toolRepository;
        }

        public async Task<List<Tool>> GetAllToolsAsync()
        {
            return await _toolRepository.GetAllToolsAsync();
        }

        public async Task<Tool> GetToolByIdAsync(int id)
        {
            var tool = await _toolRepository.GetByIdAsync(id);
            if (tool == null)
                throw new Exception($"Orodje z ID {id} ne obstaja.");
            return tool;
        }

        public async Task<Tool> CreateToolAsync(CreateToolDto toolDto, string userId)
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
                Version = "1.0", // Default version
                CreatedBy = userId,
                UpdatedBy = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _toolRepository.AddAsync(tool);
            return tool;
        }

        public async Task<Tool> UpdateToolAsync(int id, UpdateToolDto toolDto, string userId)
        {
            var tool = await GetToolByIdAsync(id);

            tool.Name = toolDto.Name;
            tool.Description = toolDto.Description;
            tool.UpdatedBy = userId;

            await _toolRepository.UpdateAsync(tool);
            return tool;
        }

        public async Task DeleteToolAsync(int id)
        {
            var tool = await GetToolByIdAsync(id);
            await _toolRepository.DeleteAsync(tool);
        }
    }
}
