using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class LoadPlanReservation
{
    public int Id { get; set; }
    public int LoadPlanId { get; set; }
    public int PickingListHeaderId { get; set; } // "PickingListId" in spec
    public int StopSequence { get; set; }
    public string ReservationStatus { get; set; } = "Reserved";

    // Navigation
    [ForeignKey(nameof(LoadPlanId))]
    public LoadPlan? LoadPlan { get; set; }

    [ForeignKey(nameof(PickingListHeaderId))]
    public PickingListHeader? PickingListHeader { get; set; }
}
