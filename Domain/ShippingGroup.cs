using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class ShippingGroup
    {
        public int Id { get; set; }
        public int BranchId { get; set; }
        public int ShippingRegionId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [ForeignKey("ShippingRegionId")]
        public ShippingRegion? ShippingRegion { get; set; }
    }
}
