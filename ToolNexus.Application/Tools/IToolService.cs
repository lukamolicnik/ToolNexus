using ToolNexus.Application.Tools.DTOs;
using ToolNexus.Domain.Tools;

namespace ToolNexus.Application.Tools
{
    public interface IToolService
    {
        Task<List<Tool>> GetAllToolsAsync();
        Task<Tool> GetToolByIdAsync(int id);
        Task<Tool> CreateToolAsync(CreateToolDto toolDto, string userId);
        Task<Tool> UpdateToolAsync(int id, UpdateToolDto toolDto, string userId);
        Task DeleteToolAsync(int id);
    }
}
