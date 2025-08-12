namespace ToolNexus.Domain.Tools
{
    using ToolNexus.Domain.ToolCategories;
    using ToolNexus.Domain.Users;

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
        public bool IsActive { get; set; } = true;
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public virtual ToolCategory? ToolCategory { get; set; }
        public virtual User? CreatedByUser { get; set; }
        public virtual User? UpdatedByUser { get; set; }

        // Computed properties
        public bool IsBelowMinimum => CurrentStock <= MinimumStock;
        public bool IsCritical => CurrentStock <= CriticalStock;
    }
}
