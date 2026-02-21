using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data
{
    public class AuditEvent
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public DateTime OccurredAtUtc { get; set; }
        public string ActorUserId { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? PayloadJson { get; set; }
    }
}
