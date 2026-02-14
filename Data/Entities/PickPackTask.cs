using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class PickPackTask
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int PickingListLineId { get; set; }
    public string Status { get; set; } = "Pending";

    [ConcurrencyCheck]
    public long Version { get; set; }

    public PickingListLine PickingListLine { get; set; } = null!;
}
