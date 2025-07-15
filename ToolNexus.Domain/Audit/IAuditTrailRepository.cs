namespace ToolNexus.Domain.Audit
{
    public interface IAuditTrailRepository
    {
        Task<IEnumerable<AuditTrail>> GetAllAsync();
        Task<IEnumerable<AuditTrail>> GetByEntityAsync(string entityType, string entityId);
        Task<IEnumerable<AuditTrail>> GetByUserAsync(Guid userId);
        Task<AuditTrail?> GetByIdAsync(Guid id);
        Task<IEnumerable<AuditTrail>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<AuditTrail>> GetByEntityTypeAsync(string entityType);
    }
}