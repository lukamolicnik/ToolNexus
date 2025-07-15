namespace ToolNexus.Domain.Audit
{
    public class AuditTrail
    {
        public Guid Id { get; set; }
        public required string EntityType { get; set; }
        public required string EntityId { get; set; }
        public AuditAction Action { get; set; }
        public Guid UserId { get; set; }
        public required string UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Changes { get; set; }
        public required string TableName { get; set; }
    }
}