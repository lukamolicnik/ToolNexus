using ToolNexus.Domain.StockAdjustments;

namespace ToolNexus.Application.StockAdjustments.DTOs;

public class StockAdjustmentDto
{
    public int Id { get; set; }
    public int ToolId { get; set; }
    public string ToolCode { get; set; } = string.Empty;
    public string ToolName { get; set; } = string.Empty;
    public StockAdjustmentType AdjustmentType { get; set; }
    public int Quantity { get; set; }
    public int PreviousStock { get; set; }
    public int NewStock { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}