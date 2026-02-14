using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class PickingListLine
{
    public int Id { get; set; }
    public int PickingListHeaderId { get; set; }
    public int BranchId { get; set; }
    public int LineNumber { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; }

    // Reserved materials stored as JSON string
    public string ReservedMaterialsJson { get; set; } = "[]";

    [ConcurrencyCheck]
    public long Version { get; set; }

    public PickingListHeader Header { get; set; } = null!;
}
