using ToolNexus.Application.DTOs;

namespace ToolNexus.Application.Interfaces;

public interface IToolCategoryService
{
    Task<ToolCategoryDto?> GetByIdAsync(int id);
    Task<IEnumerable<ToolCategoryDto>> GetAllAsync();
    Task<IEnumerable<ToolCategoryDto>> GetActiveAsync();
    Task<ToolCategoryDto> CreateAsync(CreateToolCategoryDto dto);
    Task<ToolCategoryDto> UpdateAsync(UpdateToolCategoryDto dto);
    Task DeleteAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}