namespace ToolNexus.Domain.Audit
{
    using ToolNexus.Domain.Users;

    public class AuditTrail
    {
        public Guid Id { get; set; }
        public required string EntityType { get; set; }
        public required string EntityId { get; set; }
        public AuditAction Action { get; set; }
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public DateTime Timestamp { get; set; }
        public string? Changes { get; set; }

        // Navigation property
        public virtual User User { get; set; } = null!;
    }
}