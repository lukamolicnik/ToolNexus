using Microsoft.EntityFrameworkCore;
using ToolNexus.Domain.Audit;

namespace ToolNexus.Infrastructure.Repositories
{
    public class AuditTrailRepository : IAuditTrailRepository
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public AuditTrailRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<AuditTrail>> GetAllAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.AuditTrails
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditTrail>> GetByEntityAsync(string entityType, string entityId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.AuditTrails
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditTrail>> GetByUserAsync(Guid userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.AuditTrails
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<AuditTrail?> GetByIdAsync(Guid id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.AuditTrails
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<AuditTrail>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.AuditTrails
                .Where(a => a.Timestamp >= startDate && a.Timestamp <= endDate)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditTrail>> GetByEntityTypeAsync(string entityType)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.AuditTrails
                .Where(a => a.EntityType == entityType)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}