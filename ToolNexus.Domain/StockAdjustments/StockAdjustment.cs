using ToolNexus.Domain.Tools;

namespace ToolNexus.Domain.StockAdjustments;

public class StockAdjustment
{
    public int Id { get; set; }
    
    public required int ToolId { get; set; }
    
    public required StockAdjustmentType AdjustmentType { get; set; }
    
    public required int Quantity { get; set; }
    
    public int PreviousStock { get; set; }
    
    public int NewStock { get; set; }
    
    public string? Reason { get; set; }
    
    public string? Notes { get; set; }
    
    public required string AdjustedBy { get; set; }
    
    public DateTime AdjustedAt { get; set; }
    
    // Navigation properties
    public virtual Tool? Tool { get; set; }
}

public enum StockAdjustmentType
{
    Increase,
    Decrease
}