using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class Item
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        [Required]
        public string ItemCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CoilRelationship { get; set; }
        public string UOM { get; set; } = string.Empty; // PCS or LBS

        public decimal? PoundsPerSquareFoot { get; set; }

        public string? CoilItemCode { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }
    }
}
