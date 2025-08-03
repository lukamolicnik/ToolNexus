namespace ToolNexus.Domain.Tools
{
    using ToolNexus.Domain.ToolCategories;

    public class Tool
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int? ToolCategoryId { get; set; }
        public int CurrentStock { get; set; } = 0;
        public int MinimumStock { get; set; } = 0;
        public int CriticalStock { get; set; } = 0;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public virtual ToolCategory? ToolCategory { get; set; }

        // Computed properties
        public bool IsBelowMinimum => CurrentStock <= MinimumStock;
        public bool IsCritical => CurrentStock <= CriticalStock;
    }
}
