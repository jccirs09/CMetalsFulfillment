using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class ShippingFsaRule
    {
        public int Id { get; set; }
        public int BranchId { get; set; }

        [StringLength(3)]
        public string FsaPrefix { get; set; } = string.Empty;

        public int? ShippingRegionId { get; set; }
        public int? ShippingGroupId { get; set; }

        public int Priority { get; set; }
        public bool IsActive { get; set; } = true;

        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [ForeignKey("ShippingRegionId")]
        public ShippingRegion? ShippingRegion { get; set; }

        [ForeignKey("ShippingGroupId")]
        public ShippingGroup? ShippingGroup { get; set; }
    }
}
