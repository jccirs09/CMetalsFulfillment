using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class PickingListLine : IConcurrencyAware
{
    public int Id { get; set; }
    public int PickingListHeaderId { get; set; }
    public int BranchId { get; set; }

    // Import Fields
    public int LineNumber { get; set; }
    public required string ItemCode { get; set; }
    public string? Description { get; set; }
    public string? MaterialType { get; set; } // COIL | SHEET
    public string? Uom { get; set; } // PCS | LBS
    public int QtyOrdered { get; set; }
    public decimal WeightOrderedLbs { get; set; }
    public decimal WidthIn { get; set; }
    public decimal LengthIn { get; set; }
    public string? ReservedMaterialsJson { get; set; }
    public string? LineInstructions { get; set; }

    // Status
    public string Status { get; set; } = "Pending"; // Pending, Cancelled, Completed?

    // Concurrency
    [ConcurrencyCheck]
    public long Version { get; set; }

    // Navigation
    [ForeignKey(nameof(PickingListHeaderId))]
    public PickingListHeader? Header { get; set; }

    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }
}
