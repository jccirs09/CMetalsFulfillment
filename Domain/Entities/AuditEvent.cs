using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Domain.Entities;

public class AuditEvent
{
    public int Id { get; set; }
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public int? BranchId { get; set; }

    [Required]
    public string EventType { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? EntityName { get; set; }
    public string? EntityId { get; set; }
}
