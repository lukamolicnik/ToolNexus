using ToolNexus.Application.Tools.DTOs;

namespace ToolNexus.Application.Tools
{
    public interface IToolService
    {
        Task<List<ToolDto>> GetAllToolsAsync();
        Task<ToolDto> GetToolByIdAsync(int id);
        Task<ToolDto> CreateToolAsync(CreateToolDto toolDto, string userId);
        Task<ToolDto> UpdateToolAsync(int id, UpdateToolDto toolDto, string userId);
        Task DeleteToolAsync(int id);
        Task<ToolDto> IncreaseStockAsync(IncreaseStockDto increaseStockDto);
        Task<ToolDto> DecreaseStockAsync(DecreaseStockDto decreaseStockDto);
    }
}
