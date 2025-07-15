namespace ToolNexus.Domain.Tools
{
    public interface IToolRepository
    {
        Task<List<Tool>> GetAllToolsAsync();
        Task<Tool?> GetByIdAsync(int id);
        Task<Tool?> GetByCodeAsync(string code);
        Task AddAsync(Tool tool);
        Task UpdateAsync(Tool tool);
        Task DeleteAsync(Tool tool);
    }
}
