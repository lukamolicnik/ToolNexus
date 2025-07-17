using Microsoft.EntityFrameworkCore;
using ToolNexus.Domain.ToolCategories;

namespace ToolNexus.Infrastructure.Repositories;

public class ToolCategoryRepository : IToolCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public ToolCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ToolCategory?> GetByIdAsync(int id)
    {
        return await _context.ToolCategories
            .Include(tc => tc.Tools)
            .FirstOrDefaultAsync(tc => tc.Id == id);
    }

    public async Task<IEnumerable<ToolCategory>> GetAllAsync()
    {
        return await _context.ToolCategories
            .Include(tc => tc.Tools)
            .OrderBy(tc => tc.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<ToolCategory>> GetActiveAsync()
    {
        return await _context.ToolCategories
            .Include(tc => tc.Tools)
            .Where(tc => tc.IsActive)
            .OrderBy(tc => tc.Name)
            .ToListAsync();
    }

    public async Task<ToolCategory?> GetByCodeAsync(string code)
    {
        return await _context.ToolCategories
            .Include(tc => tc.Tools)
            .FirstOrDefaultAsync(tc => tc.Code == code);
    }

    public async Task<int> AddAsync(ToolCategory category)
    {
        _context.ToolCategories.Add(category);
        await _context.SaveChangesAsync();
        return category.Id;
    }

    public async Task UpdateAsync(ToolCategory category)
    {
        _context.ToolCategories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.ToolCategories.FindAsync(id);
        if (category != null)
        {
            _context.ToolCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.ToolCategories.AnyAsync(tc => tc.Id == id);
    }

    public async Task<bool> CodeExistsAsync(string code, int? excludeId = null)
    {
        var query = _context.ToolCategories.Where(tc => tc.Code == code);
        
        if (excludeId.HasValue)
        {
            query = query.Where(tc => tc.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}