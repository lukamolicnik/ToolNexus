namespace ToolNexus.Application.StockAdjustments.DTOs;

public class PagedStockAdjustmentDto
{
    public IEnumerable<StockAdjustmentDto> Items { get; set; } = new List<StockAdjustmentDto>();
    public int TotalItems { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public class StockAdjustmentPageRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? ToolId { get; set; }
    public Domain.StockAdjustments.StockAdjustmentType? AdjustmentType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SearchTerm { get; set; }
}