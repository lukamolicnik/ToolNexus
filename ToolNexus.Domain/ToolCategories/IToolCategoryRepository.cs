namespace ToolNexus.Domain.ToolCategories;

public interface IToolCategoryRepository
{
    Task<ToolCategory?> GetByIdAsync(int id);
    Task<IEnumerable<ToolCategory>> GetAllAsync();
    Task<IEnumerable<ToolCategory>> GetActiveAsync();
    Task<ToolCategory?> GetByCodeAsync(string code);
    Task<int> AddAsync(ToolCategory category);
    Task UpdateAsync(ToolCategory category);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> CodeExistsAsync(string code, int? excludeId = null);
}