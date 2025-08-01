namespace ToolNexus.Domain.StockAdjustments;

public interface IStockAdjustmentRepository
{
    Task<List<StockAdjustment>> GetAllAsync();
    Task<List<StockAdjustment>> GetByToolIdAsync(int toolId);
    Task<StockAdjustment?> GetByIdAsync(int id);
    Task AddAsync(StockAdjustment adjustment);
    Task<List<StockAdjustment>> GetRecentAdjustmentsAsync(int count = 50);
    Task<List<StockAdjustment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<StockAdjustment>> GetDecreaseAdjustmentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<StockAdjustment>> GetByToolIdAndDateRangeAsync(int toolId, DateTime startDate, DateTime endDate);
    Task<List<StockAdjustment>> GetByCategoryAndDateRangeAsync(int categoryId, DateTime startDate, DateTime endDate);
    Task<(List<StockAdjustment> items, int totalCount)> GetPagedAsync(int page, int pageSize, int? toolId = null, StockAdjustmentType? adjustmentType = null, string? searchTerm = null);
}