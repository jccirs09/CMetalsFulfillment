using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class LoadPlan : IConcurrencyAware
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    public string Status { get; set; } = "Draft"; // Draft, Active, InTransit, Delivered, Closed
    public string? TruckId { get; set; }
    public string? DriverId { get; set; } // If internal driver or external name
    public DateTime? PlannedDepartUtc { get; set; } // Renamed from PlannedShipDate, Index this

    // Concurrency
    [ConcurrencyCheck]
    public long Version { get; set; }

    // Navigation
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }

    public ICollection<LoadPlanReservation> Reservations { get; set; } = new List<LoadPlanReservation>();
}
