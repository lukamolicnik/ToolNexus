using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using ToolNexus.Domain.Audit;

namespace ToolNexus.Infrastructure.Interceptors
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly List<AuditEntry> _auditEntries = new();

        public AuditInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            if (eventData.Context != null)
            {
                OnBeforeSaveChanges(eventData.Context);
            }
            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                OnBeforeSaveChanges(eventData.Context);
            }
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            if (eventData.Context != null)
            {
                OnAfterSaveChanges(eventData.Context).GetAwaiter().GetResult();
            }
            return base.SavedChanges(eventData, result);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData,
            int result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
            {
                await OnAfterSaveChanges(eventData.Context);
            }
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private void OnBeforeSaveChanges(DbContext context)
        {
            _auditEntries.Clear();
            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged && e.State != EntityState.Detached)
                .Where(e => e.Entity.GetType() != typeof(AuditTrail)) // Exclude AuditTrail entities
                .ToList();

            foreach (var entry in entries)
            {
                var auditEntry = new AuditEntry(entry)
                {
                    UserId = GetCurrentUserId(),
                    UserName = GetCurrentUserName()
                };

                _auditEntries.Add(auditEntry);

                // Set CreatedBy/UpdatedBy fields automatically
                var currentUser = GetCurrentUserName();
                var entityType = entry.Entity.GetType();
                
                if (entry.State == EntityState.Added)
                {
                    if (entityType.GetProperty("CreatedBy") != null)
                    {
                        entry.Property("CreatedBy").CurrentValue = currentUser;
                    }
                    if (entityType.GetProperty("UpdatedBy") != null)
                    {
                        entry.Property("UpdatedBy").CurrentValue = currentUser;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entityType.GetProperty("UpdatedBy") != null)
                    {
                        entry.Property("UpdatedBy").CurrentValue = currentUser;
                    }
                    if (entityType.GetProperty("UpdatedAt") != null)
                    {
                        entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                    }
                }

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    var propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
        }

        private async Task OnAfterSaveChanges(DbContext context)
        {
            if (_auditEntries.Count == 0)
                return;

            var auditTrails = new List<AuditTrail>();

            foreach (var auditEntry in _auditEntries)
            {
                // Handle temporary properties
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }

                var auditTrail = new AuditTrail
                {
                    Id = Guid.NewGuid(),
                    EntityType = auditEntry.EntityType,
                    EntityId = JsonSerializer.Serialize(auditEntry.KeyValues),
                    Action = auditEntry.Action,
                    UserId = auditEntry.UserId,
                    UserName = auditEntry.UserName,
                    Timestamp = DateTime.UtcNow,
                    TableName = auditEntry.TableName
                };

                var changes = new Dictionary<string, object>();
                
                if (auditEntry.OldValues.Count > 0 || auditEntry.NewValues.Count > 0)
                {
                    foreach (var key in auditEntry.OldValues.Keys.Union(auditEntry.NewValues.Keys))
                    {
                        var change = new Dictionary<string, object?>();
                        
                        if (auditEntry.OldValues.TryGetValue(key, out var oldValue))
                            change["oldValue"] = oldValue;
                        
                        if (auditEntry.NewValues.TryGetValue(key, out var newValue))
                            change["newValue"] = newValue;
                        
                        changes[key] = change;
                    }
                    
                    auditTrail.Changes = JsonSerializer.Serialize(changes);
                }

                auditTrails.Add(auditTrail);
            }

            if (auditTrails.Any())
            {
                context.Set<AuditTrail>().AddRange(auditTrails);
                await context.SaveChangesAsync();
            }

            _auditEntries.Clear();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out var intUserId))
            {
                // Convert int user ID to Guid for audit trail
                // Using a deterministic GUID based on the int ID
                var bytes = new byte[16];
                BitConverter.GetBytes(intUserId).CopyTo(bytes, 0);
                return new Guid(bytes);
            }
            return Guid.Empty;
        }

        private string GetCurrentUserName()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
            return userName ?? "System";
        }

        private class AuditEntry
        {
            public AuditEntry(EntityEntry entry)
            {
                Entry = entry;
                EntityType = entry.Entity.GetType().Name;
                TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name;
                
                Action = entry.State switch
                {
                    EntityState.Added => AuditAction.Create,
                    EntityState.Modified => AuditAction.Update,
                    EntityState.Deleted => AuditAction.Delete,
                    _ => throw new NotSupportedException($"Entity state {entry.State} is not supported for audit.")
                };
            }

            public EntityEntry Entry { get; }
            public string EntityType { get; }
            public string TableName { get; }
            public AuditAction Action { get; }
            public Guid UserId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public Dictionary<string, object?> KeyValues { get; } = new();
            public Dictionary<string, object?> OldValues { get; } = new();
            public Dictionary<string, object?> NewValues { get; } = new();
            public List<PropertyEntry> TemporaryProperties { get; } = new();
        }
    }
}