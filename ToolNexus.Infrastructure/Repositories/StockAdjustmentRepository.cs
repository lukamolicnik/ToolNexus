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
                .ThenInclude(t => t.ToolCategory)
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

    public async Task<List<StockAdjustment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // Adjust end date to include the entire day
        var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);
        return await context.StockAdjustments
            .Include(sa => sa.Tool)
                .ThenInclude(t => t.ToolCategory)
            .Where(sa => sa.AdjustedAt >= startDate && sa.AdjustedAt <= adjustedEndDate)
            .OrderBy(sa => sa.AdjustedAt)
            .ToListAsync();
    }

    public async Task<List<StockAdjustment>> GetDecreaseAdjustmentsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // Adjust end date to include the entire day
        var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);
        return await context.StockAdjustments
            .Include(sa => sa.Tool)
                .ThenInclude(t => t.ToolCategory)
            .Where(sa => sa.AdjustmentType == StockAdjustmentType.Decrease &&
                       sa.AdjustedAt >= startDate &&
                       sa.AdjustedAt <= adjustedEndDate)
            .OrderBy(sa => sa.AdjustedAt)
            .ToListAsync();
    }

    public async Task<List<StockAdjustment>> GetByToolIdAndDateRangeAsync(int toolId, DateTime startDate, DateTime endDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // Adjust end date to include the entire day
        var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);
        return await context.StockAdjustments
            .Include(sa => sa.Tool)
                .ThenInclude(t => t.ToolCategory)
            .Where(sa => sa.ToolId == toolId &&
                       sa.AdjustedAt >= startDate &&
                       sa.AdjustedAt <= adjustedEndDate)
            .OrderBy(sa => sa.AdjustedAt)
            .ToListAsync();
    }

    public async Task<List<StockAdjustment>> GetByCategoryAndDateRangeAsync(int categoryId, DateTime startDate, DateTime endDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        // Adjust end date to include the entire day
        var adjustedEndDate = endDate.Date.AddDays(1).AddTicks(-1);
        return await context.StockAdjustments
            .Include(sa => sa.Tool)
                .ThenInclude(t => t.ToolCategory)
            .Where(sa => sa.Tool.ToolCategoryId == categoryId &&
                       sa.AdjustedAt >= startDate &&
                       sa.AdjustedAt <= adjustedEndDate)
            .OrderBy(sa => sa.AdjustedAt)
            .ToListAsync();
    }

    public async Task<(List<StockAdjustment> items, int totalCount)> GetPagedAsync(int page, int pageSize, int? toolId = null, StockAdjustmentType? adjustmentType = null, string? searchTerm = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        var query = context.StockAdjustments
            .Include(sa => sa.Tool)
                .ThenInclude(t => t.ToolCategory)
            .AsQueryable();

        // Apply filters
        if (toolId.HasValue)
        {
            query = query.Where(sa => sa.ToolId == toolId.Value);
        }

        if (adjustmentType.HasValue)
        {
            query = query.Where(sa => sa.AdjustmentType == adjustmentType.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(sa => 
                sa.Tool.Name.Contains(searchTerm) ||
                sa.Tool.Code.Contains(searchTerm) ||
                sa.Reason.Contains(searchTerm) ||
                sa.Notes.Contains(searchTerm) ||
                sa.AdjustedBy.Contains(searchTerm));
        }

        if (startDate.HasValue)
        {
            query = query.Where(sa => sa.AdjustedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            // Adjust end date to include the entire day
            var adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(sa => sa.AdjustedAt <= adjustedEndDate);
        }

        // Get total count
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .OrderByDescending(sa => sa.AdjustedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}