using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class InventorySnapshotLine
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int SnapshotId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public string TagNumber { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Uom { get; set; } = string.Empty; // PCS or LBS

    public InventorySnapshot Snapshot { get; set; } = null!;

    [ConcurrencyCheck]
    public long Version { get; set; }
}
