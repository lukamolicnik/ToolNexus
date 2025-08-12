namespace ToolNexus.Application.Tools.DTOs
{
    public class ToolDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ToolCategoryId { get; set; }
        public string? ToolCategoryName { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int CriticalStock { get; set; }
        public bool IsActive { get; set; }
        public bool IsBelowMinimum { get; set; }
        public bool IsCritical { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByName { get; set; }
    }
}