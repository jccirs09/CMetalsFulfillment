using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class ShippingGroup
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    public virtual Branch Branch { get; set; } = null!;
}
