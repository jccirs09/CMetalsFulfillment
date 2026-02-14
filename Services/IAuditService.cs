using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services;

public interface IAuditService
{
    Task LogAsync(int branchId, string eventType, string entityType, string entityId, string actorUserId, string? reason = null, string? payloadJson = null);
}
