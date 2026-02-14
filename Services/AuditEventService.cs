using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CMetalsFulfillment.Services
{
    public interface IAuditEventService
    {
        Task LogAsync(int branchId, string eventType, string entityType, string entityId, string actorUserId, string? reason = null, object? payload = null);
    }

    public class AuditEventService(IDbContextFactory<ApplicationDbContext> dbFactory) : IAuditEventService
    {
        public async Task LogAsync(int branchId, string eventType, string entityType, string entityId, string actorUserId, string? reason = null, object? payload = null)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            var auditEvent = new AuditEvent
            {
                BranchId = branchId,
                EventType = eventType,
                EntityType = entityType,
                EntityId = entityId,
                OccurredAtUtc = DateTime.UtcNow,
                ActorUserId = actorUserId,
                Reason = reason,
                PayloadJson = payload != null ? JsonSerializer.Serialize(payload) : null
            };

            context.AuditEvents.Add(auditEvent);
            await context.SaveChangesAsync();
        }
    }
}
