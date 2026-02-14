using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class InventorySnapshot
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int SnapshotHeaderId { get; set; } // Grouping for a single upload
    [Required]
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal WeightLbs { get; set; }
    public string Uom { get; set; } = string.Empty;
    public DateTime SnapshotDate { get; set; } = DateTime.UtcNow;

    public virtual Branch Branch { get; set; } = null!;
    public virtual InventorySnapshotHeader Header { get; set; } = null!;
}
