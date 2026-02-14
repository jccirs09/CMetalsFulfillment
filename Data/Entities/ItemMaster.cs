using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class ItemMaster
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? CoilRelationship { get; set; }
    public decimal? Ppsf { get; set; }
    public string Uom { get; set; } = "LBS";

    [ConcurrencyCheck]
    public long Version { get; set; }
}
