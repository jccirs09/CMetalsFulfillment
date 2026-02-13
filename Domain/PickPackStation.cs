using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class PickPackStation
    {
        public int PickPackStationId { get; set; }
        public int BranchId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }
    }
}
