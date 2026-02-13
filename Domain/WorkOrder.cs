using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Domain
{
    public class WorkOrder
    {
        [Key]
        public int WorkOrderId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        [MaxLength(20)]
        public string WorkOrderNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Scheduled, InProgress, Paused, Completed, Cancelled

        public DateTime? ScheduledDate { get; set; }
        public int Sequence { get; set; }

        public string? LastMovedByUserId { get; set; }
        public DateTime? LastMovedAtUtc { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }

        [ForeignKey(nameof(MachineId))]
        public Machine? Machine { get; set; }
    }

    public class WorkOrderInputCoil
    {
        [Key]
        public int Id { get; set; }

        public int WorkOrderId { get; set; }

        [Required]
        [MaxLength(50)]
        public string CoilTagNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        public decimal WeightLbs { get; set; }

        [ForeignKey(nameof(WorkOrderId))]
        public WorkOrder? WorkOrder { get; set; }
    }

    public class WorkOrderOutputLine
    {
        [Key]
        public int Id { get; set; }

        public int WorkOrderId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SourceType { get; set; } = "Stock"; // "PLLine" or "Stock"

        public int? PickingListLineId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        public decimal PlannedQty { get; set; }

        [MaxLength(10)]
        public string UOM { get; set; } = "PCS";

        public decimal ProducedQty { get; set; }

        [ForeignKey(nameof(WorkOrderId))]
        public WorkOrder? WorkOrder { get; set; }

        [ForeignKey(nameof(PickingListLineId))]
        public PickingListLine? PickingListLine { get; set; }
    }
}
