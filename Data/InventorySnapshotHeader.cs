namespace CMetalsFulfillment.Data;

public class InventorySnapshotHeader
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    public string UploadedByUserId { get; set; } = string.Empty;

    public virtual Branch Branch { get; set; } = null!;
    public virtual ApplicationUser UploadedByUser { get; set; } = null!;
    public virtual ICollection<InventorySnapshot> Lines { get; set; } = new List<InventorySnapshot>();
}
