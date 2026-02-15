using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class ShiftTemplate
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(20)]
    public string ShiftCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public TimeOnly StartLocalTime { get; set; }

    [Required]
    public TimeOnly EndLocalTime { get; set; }

    public int BreakMinutes { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public bool IsOvernight => EndLocalTime < StartLocalTime;
}
