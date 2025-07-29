namespace ToolNexus.Domain.StockAdjustments;

public interface IStockAdjustmentRepository
{
    Task<List<StockAdjustment>> GetAllAsync();
    Task<List<StockAdjustment>> GetByToolIdAsync(int toolId);
    Task<StockAdjustment?> GetByIdAsync(int id);
    Task AddAsync(StockAdjustment adjustment);
    Task<List<StockAdjustment>> GetRecentAdjustmentsAsync(int count = 50);
}