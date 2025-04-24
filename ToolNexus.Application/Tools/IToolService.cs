using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolNexus.Domain.Tools;

namespace ToolNexus.Application.Tools
{
    public interface IToolService
    {
        Task<List<Tool>> GetAllToolsAsync();
    }
}
