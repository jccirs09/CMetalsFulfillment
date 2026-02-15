using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class ItemMaster
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Sku { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Uom { get; set; } = "LB"; // Default to LB/EA/FT

    [Range(0, double.MaxValue)]
    public decimal WeightPerUnit { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
