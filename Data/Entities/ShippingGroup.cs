using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class ShippingGroup
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(20)]
    public string GroupCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string GroupName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
