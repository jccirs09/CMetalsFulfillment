using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class PickingListHeader
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string PickingListNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";

    [ConcurrencyCheck]
    public long Version { get; set; }
}
