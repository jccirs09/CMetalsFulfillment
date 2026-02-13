using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class LogisticsTag
    {
        [Key]
        [MaxLength(8)]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Tag must be exactly 8 digits")]
        public string TagNumber { get; set; } = string.Empty;

        [Required]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SourceType { get; set; } = "CtlWO"; // CtlWO, SlitterWO, PickPack

        public int SourceRefId { get; set; } // WorkOrderId or PickPackTaskId

        [MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [MaxLength(10)]
        public string UOM { get; set; } = "PCS";

        public decimal WeightLbs { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Available"; // Available, Assigned, Shipped, Received

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }
    }
}
