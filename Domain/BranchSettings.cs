using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class BranchSettings
    {
        [Key]
        public int BranchId { get; set; }
        public DateTime? SetupCompletedAtUtc { get; set; }

        public int? DefaultPickPackStationId { get; set; }

        // Navigation properties
        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }

        [ForeignKey("DefaultPickPackStationId")]
        public PickPackStation? DefaultPickPackStation { get; set; }
    }
}
