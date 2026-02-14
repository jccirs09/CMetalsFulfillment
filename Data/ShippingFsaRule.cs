using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class ShippingFsaRule
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    [Required]
    public string PostalCodePrefix { get; set; } = string.Empty;
    public int? RegionId { get; set; }
    public int? GroupId { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual ShippingRegion? Region { get; set; }
    public virtual ShippingGroup? Group { get; set; }
}
