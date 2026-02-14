using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class AuditService : IAuditService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public AuditService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task LogAsync(int branchId, string eventType, string entityType, string entityId, string actorUserId, string? reason = null, string? payloadJson = null)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        var audit = new AuditEvent
        {
            BranchId = branchId,
            EventType = eventType,
            EntityType = entityType,
            EntityId = entityId,
            ActorUserId = actorUserId,
            Reason = reason,
            PayloadJson = payloadJson,
            OccurredAtUtc = DateTime.UtcNow
        };

        db.AuditEvents.Add(audit);
        await db.SaveChangesAsync();
    }
}
