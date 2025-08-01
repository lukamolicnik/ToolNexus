using ToolNexus.Application.StockAdjustments.DTOs;

namespace ToolNexus.Application.StockAdjustments;

public interface IStockAdjustmentService
{
    Task<List<StockAdjustmentDto>> GetAllAsync();
    Task<List<StockAdjustmentDto>> GetByToolIdAsync(int toolId);
    Task<StockAdjustmentDto?> GetByIdAsync(int id);
    Task<List<StockAdjustmentDto>> GetRecentAdjustmentsAsync(int count = 50);
    Task<PagedStockAdjustmentDto> GetPagedAsync(StockAdjustmentPageRequest request);
}