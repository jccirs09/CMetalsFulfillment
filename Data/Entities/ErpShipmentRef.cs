using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class ErpShipmentRef : IConcurrencyAware
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    public required string ErpPackingListNumber { get; set; }
    public int PickingListHeaderId { get; set; }

    public DateTime? ShipDateLocal { get; set; }
    public string Status { get; set; } = "Open"; // Open, Closed?

    // Concurrency
    [ConcurrencyCheck]
    public long Version { get; set; }

    // Navigation
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }

    [ForeignKey(nameof(PickingListHeaderId))]
    public PickingListHeader? PickingListHeader { get; set; }
}
