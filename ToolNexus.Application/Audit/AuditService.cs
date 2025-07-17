using System.Text.Json;
using ToolNexus.Application.Audit.DTOs;
using ToolNexus.Domain.Audit;

namespace ToolNexus.Application.Audit
{
    public class AuditService : IAuditService
    {
        private readonly IAuditTrailRepository _auditTrailRepository;

        public AuditService(IAuditTrailRepository auditTrailRepository)
        {
            _auditTrailRepository = auditTrailRepository;
        }

        public async Task<IEnumerable<AuditTrailDto>> GetAllAsync()
        {
            var auditTrails = await _auditTrailRepository.GetAllAsync();
            return auditTrails.Select(MapToDto);
        }

        public async Task<PagedAuditTrailDto> GetPagedAsync(AuditTrailPageRequest request)
        {
            var (items, totalCount) = await _auditTrailRepository.GetPagedAsync(
                request.Page,
                request.PageSize,
                request.EntityType,
                request.EntityId,
                request.UserId,
                request.StartDate,
                request.EndDate,
                request.SearchTerm);

            return new PagedAuditTrailDto
            {
                Items = items.Select(MapToDto),
                TotalItems = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<IEnumerable<AuditTrailDto>> GetByEntityAsync(string entityType, string entityId)
        {
            var auditTrails = await _auditTrailRepository.GetByEntityAsync(entityType, entityId);
            return auditTrails.Select(MapToDto);
        }

        public async Task<IEnumerable<AuditTrailDto>> GetByUserAsync(Guid userId)
        {
            var auditTrails = await _auditTrailRepository.GetByUserAsync(userId);
            return auditTrails.Select(MapToDto);
        }

        public async Task<AuditTrailDto?> GetByIdAsync(Guid id)
        {
            var auditTrail = await _auditTrailRepository.GetByIdAsync(id);
            return auditTrail != null ? MapToDto(auditTrail) : null;
        }

        public async Task<IEnumerable<AuditTrailDto>> GetByFilterAsync(AuditTrailFilterDto filter)
        {
            var allAudits = await _auditTrailRepository.GetAllAsync();
            var query = allAudits.AsQueryable();

            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(a => a.EntityType == filter.EntityType);

            if (!string.IsNullOrEmpty(filter.EntityId))
                query = query.Where(a => a.EntityId == filter.EntityId);

            if (filter.UserId.HasValue)
                query = query.Where(a => a.UserId == filter.UserId.Value);

            if (filter.StartDate.HasValue)
                query = query.Where(a => a.Timestamp >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(a => a.Timestamp <= filter.EndDate.Value);

            if (!string.IsNullOrEmpty(filter.Action))
                query = query.Where(a => a.Action.ToString() == filter.Action);

            // Apply pagination
            var paginatedResults = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            return paginatedResults.Select(MapToDto);
        }

        public async Task<Dictionary<string, int>> GetAuditStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var auditTrails = await _auditTrailRepository.GetAllAsync();
            
            if (startDate.HasValue)
                auditTrails = auditTrails.Where(a => a.Timestamp >= startDate.Value);
            
            if (endDate.HasValue)
                auditTrails = auditTrails.Where(a => a.Timestamp <= endDate.Value);

            var statistics = new Dictionary<string, int>
            {
                ["TotalChanges"] = auditTrails.Count(),
                ["Creates"] = auditTrails.Count(a => a.Action == AuditAction.Create),
                ["Updates"] = auditTrails.Count(a => a.Action == AuditAction.Update),
                ["Deletes"] = auditTrails.Count(a => a.Action == AuditAction.Delete),
                ["UniqueUsers"] = auditTrails.Select(a => a.UserId).Distinct().Count(),
                ["UniqueEntities"] = auditTrails.Select(a => new { a.EntityType, a.EntityId }).Distinct().Count()
            };

            // Count by entity type
            var entityGroups = auditTrails.GroupBy(a => a.EntityType);
            foreach (var group in entityGroups)
            {
                statistics[$"{group.Key}Changes"] = group.Count();
            }

            return statistics;
        }

        private AuditTrailDto MapToDto(AuditTrail auditTrail)
        {
            var dto = new AuditTrailDto
            {
                Id = auditTrail.Id,
                EntityType = auditTrail.EntityType,
                EntityId = auditTrail.EntityId,
                Action = auditTrail.Action.ToString(),
                UserId = auditTrail.UserId,
                UserName = auditTrail.UserName,
                Timestamp = auditTrail.Timestamp,
                Changes = auditTrail.Changes,
                TableName = auditTrail.TableName
            };

            // Parse changes JSON into ChangeDetails
            if (!string.IsNullOrEmpty(auditTrail.Changes))
            {
                try
                {
                    var changesDict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(auditTrail.Changes);
                    if (changesDict != null)
                    {
                        dto.ChangeDetails = new List<ChangeDetail>();
                        foreach (var change in changesDict)
                        {
                            var detail = new ChangeDetail
                            {
                                PropertyName = change.Key
                            };

                            if (change.Value.TryGetValue("oldValue", out var oldValue))
                                detail.OldValue = oldValue;

                            if (change.Value.TryGetValue("newValue", out var newValue))
                                detail.NewValue = newValue;

                            dto.ChangeDetails.Add(detail);
                        }
                    }
                }
                catch (JsonException)
                {
                    // If parsing fails, leave ChangeDetails as null
                }
            }

            return dto;
        }
    }
}