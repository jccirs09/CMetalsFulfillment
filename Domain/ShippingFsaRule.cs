using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class ShippingFsaRule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }

        [Required]
        [MaxLength(3)]
        [Column(TypeName = "nchar(3)")]
        public string FsaPrefix { get; set; } = string.Empty;

        [Required]
        public int ShippingRegionId { get; set; }

        public int? ShippingGroupId { get; set; }

        public int Priority { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }

        [ForeignKey(nameof(ShippingRegionId))]
        public ShippingRegion? Region { get; set; }

        [ForeignKey(nameof(ShippingGroupId))]
        public ShippingGroup? Group { get; set; }
    }
}
