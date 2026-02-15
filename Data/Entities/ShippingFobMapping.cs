using System.ComponentModel.DataAnnotations;
using CMetalsFulfillment.Data.Enums;

namespace CMetalsFulfillment.Data.Entities;

public class ShippingFobMapping
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    [MaxLength(100)]
    public string FobToken { get; set; } = string.Empty;

    [Required]
    public ShippingMethod ShippingMethod { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
