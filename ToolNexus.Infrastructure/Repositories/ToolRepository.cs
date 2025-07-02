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
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        
        public ToolRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        public async Task<List<Tool>> GetAllToolsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Tools.ToListAsync();
        }
    }
}
