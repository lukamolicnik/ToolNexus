namespace ToolNexus.Application.Audit.DTOs
{
    public class AuditTrailFilterDto
    {
        public string? EntityType { get; set; }
        public string? EntityId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Action { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}