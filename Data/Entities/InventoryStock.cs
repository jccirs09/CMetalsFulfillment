using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class InventoryStock
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string TagNumber { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Uom { get; set; } = string.Empty;
    public int LastSnapshotId { get; set; }

    [ConcurrencyCheck]
    public long Version { get; set; }
}
