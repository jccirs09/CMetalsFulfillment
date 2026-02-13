using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Domain
{
    public class PickingList
    {
        [Key]
        public Guid PickingListUid { get; set; }

        [Required]
        public int ImportBranchId { get; set; }

        [Required]
        [MaxLength(50)]
        public string PickingListNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ShipToName { get; set; }

        [MaxLength(200)]
        public string? ShipToAddress { get; set; }

        [MaxLength(50)]
        public string? ShipToCity { get; set; }

        [MaxLength(20)]
        public string? ShipToState { get; set; }

        [MaxLength(20)]
        public string? ShipToZip { get; set; }

        [MaxLength(50)]
        public string? FOBPoint { get; set; }

        public DateTime? ShipDateLocal { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Open"; // Open, Cancelled, Shipped

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(ImportBranchId))]
        public Branch? Branch { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser? CreatedByUser { get; set; }
    }

    public class PickingListLine
    {
        [Key]
        public int Id { get; set; }

        public Guid PickingListUid { get; set; }

        public int LineNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        public decimal QtyOrdered { get; set; }

        [MaxLength(10)]
        public string UOM { get; set; } = "PCS";

        [Required]
        [MaxLength(20)]
        public string FulfillmentKind { get; set; } = "PickPack"; // CTL, Slitter, PickPack

        public int? AssignedPickPackStationId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Open"; // Open, PendingCancel, Cancelled, Completed

        [ForeignKey(nameof(PickingListUid))]
        public PickingList? PickingList { get; set; }

        [ForeignKey(nameof(AssignedPickPackStationId))]
        public PickPackStation? AssignedStation { get; set; }
    }

    public class PickingListEvent
    {
        [Key]
        public int Id { get; set; }

        public Guid PickingListUid { get; set; }

        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string EventType { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Details { get; set; }

        [ForeignKey(nameof(PickingListUid))]
        public PickingList? PickingList { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }
    }
}
