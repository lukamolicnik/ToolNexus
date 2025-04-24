using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolNexus.Domain.Tools
{
    public interface IToolRepository
    {
        Task<List<Tool>> GetAllToolsAsync();
    }
}
