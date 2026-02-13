using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class BranchSettings
    {
        [Key]
        [ForeignKey(nameof(Branch))]
        public int BranchId { get; set; }

        public DateTime? SetupCompletedAtUtc { get; set; }

        public int? DefaultPickPackStationId { get; set; }

        public Branch? Branch { get; set; }
    }
}
