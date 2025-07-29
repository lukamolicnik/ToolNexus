using Microsoft.EntityFrameworkCore;
using ToolNexus.Domain.StockAdjustments;

namespace ToolNexus.Infrastructure.Repositories;

public class StockAdjustmentRepository : IStockAdjustmentRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public StockAdjustmentRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<List<StockAdjustment>> GetAllAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockAdjustments
            .Include(sa => sa.Tool)
            .OrderByDescending(sa => sa.AdjustedAt)
            .ToListAsync();
    }

    public async Task<List<StockAdjustment>> GetByToolIdAsync(int toolId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockAdjustments
            .Include(sa => sa.Tool)
            .Where(sa => sa.ToolId == toolId)
            .OrderByDescending(sa => sa.AdjustedAt)
            .ToListAsync();
    }

    public async Task<StockAdjustment?> GetByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockAdjustments
            .Include(sa => sa.Tool)
            .FirstOrDefaultAsync(sa => sa.Id == id);
    }

    public async Task AddAsync(StockAdjustment adjustment)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.StockAdjustments.Add(adjustment);
        await context.SaveChangesAsync();
    }

    public async Task<List<StockAdjustment>> GetRecentAdjustmentsAsync(int count = 50)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockAdjustments
            .Include(sa => sa.Tool)
            .OrderByDescending(sa => sa.AdjustedAt)
            .Take(count)
            .ToListAsync();
    }
}