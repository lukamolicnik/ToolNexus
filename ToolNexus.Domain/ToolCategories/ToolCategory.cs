using ToolNexus.Domain.Tools;
using ToolNexus.Domain.Users;

namespace ToolNexus.Domain.ToolCategories;

public class ToolCategory
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();
    public virtual User? CreatedByUser { get; set; }
    public virtual User? UpdatedByUser { get; set; }
}