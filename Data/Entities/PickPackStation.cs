using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class PickPackStation
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(20)]
    public string StationCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string StationName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? Notes { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
