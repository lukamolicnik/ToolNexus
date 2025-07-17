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
            return await context.Tools.Include(t => t.ToolCategory).ToListAsync();
        }

        public async Task<Tool?> GetByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Tools.Include(t => t.ToolCategory).FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tool?> GetByCodeAsync(string code)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Tools.Include(t => t.ToolCategory).FirstOrDefaultAsync(t => t.Code == code);
        }

        public async Task AddAsync(Tool tool)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Tools.Add(tool);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tool tool)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var existingTool = await context.Tools.Include(t => t.ToolCategory).FirstOrDefaultAsync(t => t.Id == tool.Id);
            if (existingTool != null)
            {
                // Update only specific properties
                existingTool.Name = tool.Name;
                existingTool.Description = tool.Description;
                existingTool.UpdatedBy = tool.UpdatedBy;
                existingTool.UpdatedAt = DateTime.UtcNow;
                
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Tool tool)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Tools.Remove(tool);
            await context.SaveChangesAsync();
        }
    }
}
