namespace CMetalsFulfillment.Data.Entities;

public class ShippingFsaRule
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string FsaPrefix { get; set; } = string.Empty;
    public int ShippingRegionId { get; set; }
    public int ShippingGroupId { get; set; }

    public ShippingRegion ShippingRegion { get; set; } = null!;
    public ShippingGroup ShippingGroup { get; set; } = null!;
}
