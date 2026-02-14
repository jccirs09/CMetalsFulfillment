using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class AuditEvent
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    public required string EventType { get; set; }
    public required string EntityType { get; set; }
    public string? EntityId { get; set; } // Could be int or string
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public required string ActorUserId { get; set; }
    public string? Reason { get; set; }
    public string? PayloadJson { get; set; }

    // Navigation
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }

    [ForeignKey(nameof(ActorUserId))]
    public ApplicationUser? ActorUser { get; set; }
}
