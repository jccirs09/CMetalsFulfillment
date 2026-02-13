using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Domain
{
    public class WorkOrderProductionRecord
    {
        [Key]
        public int Id { get; set; }

        public int WorkOrderId { get; set; }
        public int WorkOrderOutputLineId { get; set; }

        [Required]
        [MaxLength(8)]
        public string ProducedTagNumber { get; set; } = string.Empty;

        public decimal ProducedWeightLbs { get; set; }
        public DateTime ProducedAtUtc { get; set; } = DateTime.UtcNow;
        public string ProducedByUserId { get; set; } = string.Empty;

        [ForeignKey(nameof(WorkOrderId))]
        public WorkOrder? WorkOrder { get; set; }

        [ForeignKey(nameof(WorkOrderOutputLineId))]
        public WorkOrderOutputLine? OutputLine { get; set; }
    }

    public class WorkOrderEvent
    {
        [Key]
        public int Id { get; set; }

        public int WorkOrderId { get; set; }
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
        public string UserId { get; set; } = string.Empty;

        [MaxLength(50)]
        public string EventType { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Details { get; set; }

        [ForeignKey(nameof(WorkOrderId))]
        public WorkOrder? WorkOrder { get; set; }
    }
}
