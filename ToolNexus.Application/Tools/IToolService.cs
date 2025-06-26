using ToolNexus.Domain.Tools;

namespace ToolNexus.Application.Tools
{
    public interface IToolService
    {
        Task<List<Tool>> GetAllToolsAsync();
    }
}
