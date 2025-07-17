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
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public virtual ToolCategory? ToolCategory { get; set; }
    }
}
