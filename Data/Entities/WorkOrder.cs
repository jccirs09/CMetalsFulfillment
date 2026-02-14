using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class WorkOrder : IConcurrencyAware
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    // Links to source
    public int? PickingListLineId { get; set; } // If derived from picking list

    // Scheduling
    public DateTime? PlannedStartUtc { get; set; } // Index this
    public DateTime? PlannedEndUtc { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public string? MachineId { get; set; } // Index this

    // Status
    public string Status { get; set; } = "Planned"; // Planned, InProgress, Completed, Cancelled

    // Concurrency
    [ConcurrencyCheck]
    public long Version { get; set; }

    // Navigation
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }

    [ForeignKey(nameof(PickingListLineId))]
    public PickingListLine? PickingListLine { get; set; }
}
