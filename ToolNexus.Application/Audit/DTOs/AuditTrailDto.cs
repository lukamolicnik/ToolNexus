using ToolNexus.Domain.Audit;

namespace ToolNexus.Application.Audit.DTOs
{
    public class AuditTrailDto
    {
        public Guid Id { get; set; }
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string? Changes { get; set; }
        public string TableName { get; set; } = string.Empty;
        public List<ChangeDetail>? ChangeDetails { get; set; }
    }

    public class ChangeDetail
    {
        public string PropertyName { get; set; } = string.Empty;
        public object? OldValue { get; set; }
        public object? NewValue { get; set; }
    }
}