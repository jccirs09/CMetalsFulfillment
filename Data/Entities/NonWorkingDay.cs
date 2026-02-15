using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class NonWorkingDay
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    public DateOnly DateLocal { get; set; }

    [Required]
    public string Reason { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
