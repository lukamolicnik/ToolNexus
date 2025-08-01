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

        public async Task<(IEnumerable<AuditTrail> Items, int TotalCount)> GetPagedAsync(
            int page, 
            int pageSize, 
            string? entityType = null,
            string? entityId = null,
            string? action = null,
            Guid? userId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string? searchTerm = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            
            var query = context.AuditTrails.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(entityType))
                query = query.Where(a => a.EntityType == entityType);

            if (!string.IsNullOrEmpty(entityId))
                query = query.Where(a => a.EntityId == entityId);
                
            if (!string.IsNullOrEmpty(action))
            {
                if (Enum.TryParse<AuditAction>(action, out var actionEnum))
                    query = query.Where(a => a.Action == actionEnum);
            }

            if (userId.HasValue)
                query = query.Where(a => a.UserId == userId.Value);

            if (startDate.HasValue)
                query = query.Where(a => a.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Timestamp <= endDate.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => 
                    a.EntityType.Contains(searchTerm) ||
                    a.UserName.Contains(searchTerm) ||
                    a.TableName.Contains(searchTerm) ||
                    (a.Changes != null && a.Changes.Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
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