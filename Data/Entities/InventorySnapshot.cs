using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class InventorySnapshot
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
    public string UploadedByUserId { get; set; } = string.Empty;
    public string Filename { get; set; } = string.Empty;

    [ConcurrencyCheck]
    public long Version { get; set; }
}
