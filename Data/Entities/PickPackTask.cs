using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class PickPackTask : IConcurrencyAware
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    public int PickingListLineId { get; set; }

    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed

    // Concurrency
    [ConcurrencyCheck]
    public long Version { get; set; }

    // Navigation
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }

    [ForeignKey(nameof(PickingListLineId))]
    public PickingListLine? PickingListLine { get; set; }
}
