using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class ShippingRegion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }
    }

    public class ShippingGroup
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        public int ShippingRegionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }

        [ForeignKey(nameof(ShippingRegionId))]
        public ShippingRegion? Region { get; set; }
    }
}
