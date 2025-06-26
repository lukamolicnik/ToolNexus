namespace ToolNexus.Domain.Tools
{
    public interface IToolRepository
    {
        Task<List<Tool>> GetAllToolsAsync();
    }
}
