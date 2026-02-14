using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities
{
    public class AuditEvent
    {
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public string EventType { get; set; } = string.Empty;

        [Required]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        public string EntityId { get; set; } = string.Empty;

        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

        [Required]
        public string ActorUserId { get; set; } = string.Empty;

        public string? Reason { get; set; }

        [Required]
        public string PayloadJson { get; set; } = "{}";
    }
}
