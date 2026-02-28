using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data
{
    public class AuditEvent
    {
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string EventType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EntityType { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string EntityId { get; set; } = string.Empty;

        public DateTime OccurredAtUtc { get; set; }

        [Required]
        public string ActorUserId { get; set; } = string.Empty;
        public ApplicationUser ActorUser { get; set; } = null!;

        public string? Reason { get; set; }

        [Required]
        public string PayloadJson { get; set; } = string.Empty;
    }
}
