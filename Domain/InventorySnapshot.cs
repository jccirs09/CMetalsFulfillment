using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Domain
{
    public class InventorySnapshot
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }

        public DateTime ImportedAtUtc { get; set; } = DateTime.UtcNow;

        [Required]
        public string ImportedByUserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string SourceFileName { get; set; } = string.Empty;

        public int TotalRows { get; set; }
        public int MatchedRows { get; set; }
        public int UnmatchedRows { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }

        [ForeignKey(nameof(ImportedByUserId))]
        public ApplicationUser? ImportedByUser { get; set; }
    }

    public class InventorySnapshotLine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SnapshotId { get; set; }

        public int LineNo { get; set; }

        [MaxLength(50)]
        public string? ItemCode { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Size { get; set; }

        [MaxLength(20)]
        public string? Mode { get; set; }

        [MaxLength(20)]
        public string? TagNumber { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; }

        [MaxLength(20)]
        public string? Correctable { get; set; }

        [MaxLength(50)]
        public string? SnapshotLocation { get; set; }

        [MaxLength(50)]
        public string? CountLocation { get; set; }

        public decimal? SnapshotValue { get; set; }
        public decimal? CountValue { get; set; }

        [MaxLength(100)]
        public string? Exception { get; set; }

        [MaxLength(10)]
        public string? UOM { get; set; } // Only PCS or LBS

        public decimal? Amount { get; set; }

        [MaxLength(50)]
        public string MatchStatus { get; set; } = "Unmatched"; // MissingItemCode, InvalidUom, Matched, Unmatched

        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? MatchedItemId { get; set; }

        [ForeignKey(nameof(SnapshotId))]
        public InventorySnapshot? Snapshot { get; set; }

        [ForeignKey(nameof(MatchedItemId))]
        public Item? MatchedItem { get; set; }
    }
}
