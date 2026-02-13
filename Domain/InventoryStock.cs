using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class InventoryStock
    {
        [Key]
        public int InventoryStockId { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        public decimal? QuantityOnHand { get; set; }
        public decimal? WeightOnHand { get; set; }

        public DateTime LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }
    }
}
