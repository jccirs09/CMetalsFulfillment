using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class NonWorkingDayOverride
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    public DateOnly DateLocal { get; set; }

    [Required]
    [MaxLength(50)]
    public string OverrideType { get; set; } = "OvertimeEnabled";

    [Required]
    public string Reason { get; set; } = string.Empty;

    [Required]
    public string ApprovedByUserId { get; set; } = string.Empty;

    public DateTime ApprovedAtUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public int AuditEventId { get; set; }
}
