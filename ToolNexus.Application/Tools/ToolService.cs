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
    }
}
