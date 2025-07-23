using ToolNexus.Application.ToolCategories.DTOs;

namespace ToolNexus.Application.ToolCategories;

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