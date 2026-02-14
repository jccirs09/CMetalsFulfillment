using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public class AuditService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        public async Task LogAsync(int branchId, string eventType, string entityType, string entityId, string actorUserId, string payloadJson, string? reason = null)
        {
            using var context = await dbContextFactory.CreateDbContextAsync();
            var auditEvent = new AuditEvent
            {
                BranchId = branchId,
                EventType = eventType,
                EntityType = entityType,
                EntityId = entityId,
                ActorUserId = actorUserId,
                PayloadJson = payloadJson,
                Reason = reason,
                OccurredAtUtc = DateTime.UtcNow
            };

            context.AuditEvents.Add(auditEvent);
            await context.SaveChangesAsync();
        }
    }
}
