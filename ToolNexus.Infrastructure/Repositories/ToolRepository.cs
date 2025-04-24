using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolNexus.Domain.Tools;

namespace ToolNexus.Infrastructure.Repositories
{
    public class ToolRepository : IToolRepository
    {
        private readonly ApplicationDbContext _context;
        public ToolRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Tool>> GetAllToolsAsync()
        {
            return await _context.Tools.ToListAsync();
        }
    }
}
