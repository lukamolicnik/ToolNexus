using ToolNexus.Domain.Tools;
using ToolNexus.Domain.Users;

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
    
    public required int CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public virtual Tool? Tool { get; set; }
    public virtual User? CreatedByUser { get; set; }
}

public enum StockAdjustmentType
{
    Increase,
    Decrease
}