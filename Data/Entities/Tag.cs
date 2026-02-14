using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class Tag : IConcurrencyAware
{
    public int Id { get; set; }
    public int BranchId { get; set; }

    public required string TagNumber { get; set; } // 8 digits (Unique Index)

    // Status (Available, Assigned, Shipped, Hold, Voided)
    public string Status { get; set; } = "Available";

    // Metadata
    public decimal WeightLbs { get; set; }
    public string? ItemCode { get; set; }
    public string? Uom { get; set; }

    // Source Tracking
    public string? SourceType { get; set; } // e.g. "WorkOrder", "Import"
    public string? SourceRefId { get; set; }

    // Destination Control
    public int? AllowedDestinationBranchId { get; set; }

    // Deletion
    public bool SoftDeleted { get; set; }

    // Concurrency
    [ConcurrencyCheck]
    public long Version { get; set; }

    // Navigation
    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }

    [ForeignKey(nameof(AllowedDestinationBranchId))]
    public Branch? AllowedDestinationBranch { get; set; }
}
