using System;

namespace CMetalsFulfillment.Data.Entities;

public class AuditEvent
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public string ActorUserId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = string.Empty;
}
