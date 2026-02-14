using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class AuditEvent
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(50)]
    public string EventType { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string EntityType { get; set; } = string.Empty;

    [Required]
    public string EntityId { get; set; } = string.Empty;

    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public string ActorUserId { get; set; } = string.Empty;

    public string? Reason { get; set; }

    public string? PayloadJson { get; set; }
}
