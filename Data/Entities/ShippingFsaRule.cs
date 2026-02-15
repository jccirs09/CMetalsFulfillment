using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class ShippingFsaRule
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(3)]
    public string FsaPrefix { get; set; } = string.Empty;

    [Required]
    public int RegionId { get; set; }

    [ForeignKey(nameof(RegionId))]
    public ShippingRegion Region { get; set; } = default!;

    [Required]
    public int GroupId { get; set; }

    [ForeignKey(nameof(GroupId))]
    public ShippingGroup Group { get; set; } = default!;

    public int Priority { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
