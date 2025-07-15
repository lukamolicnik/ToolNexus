using ToolNexus.Application.Audit.DTOs;

namespace ToolNexus.Application.Audit
{
    public interface IAuditService
    {
        Task<IEnumerable<AuditTrailDto>> GetAllAsync();
        Task<IEnumerable<AuditTrailDto>> GetByEntityAsync(string entityType, string entityId);
        Task<IEnumerable<AuditTrailDto>> GetByUserAsync(Guid userId);
        Task<AuditTrailDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<AuditTrailDto>> GetByFilterAsync(AuditTrailFilterDto filter);
        Task<Dictionary<string, int>> GetAuditStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}