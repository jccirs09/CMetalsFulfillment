using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Domain
{
    [Index(nameof(BranchId), nameof(ItemCode), IsUnique = true)]
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? CoilRelationship { get; set; }

        [MaxLength(10)]
        public string UOM { get; set; } = "PCS"; // "PCS" or "LBS"

        public decimal? PoundsPerSquareFoot { get; set; }

        [MaxLength(50)]
        public string? CoilItemCode { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }
    }
}
