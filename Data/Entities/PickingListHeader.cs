using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class PickingListHeader
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    // Import Fields
    public required string PickingListNumber { get; set; } // Unique Index with BranchId
    public DateTime? PrintDate { get; set; }
    public DateTime? ShipDate { get; set; } // Index this
    public string? Buyer { get; set; }
    public string? SalesRep { get; set; }
    public string? ShipVia { get; set; }
    public string? FobPoint { get; set; }
    public string? SoldTo { get; set; }
    public string? ShipTo { get; set; }
    public decimal TotalWeightLbs { get; set; }
    public string? OrderInstructions { get; set; }

    // Additional Logic Fields
    public string? ShippingMethod { get; set; } // Derived from FOB_POINT
    public string? ShippingRegion { get; set; } // Derived via FSA rules
    public string? ShippingGroup { get; set; }

    // Audit/Status
    public string Status { get; set; } = "Imported"; // Imported, Planned, Picked, Staged, Loaded, Shipped, Cancelled
    public DateTime ImportedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }

    public ICollection<PickingListLine> Lines { get; set; } = new List<PickingListLine>();
}
