using CMetalsFulfillment.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities
{
    public class LoadPlan : IConcurrencyAware
    {
        public int Id { get; set; }
        [Required]
        public int BranchId { get; set; }

        [ConcurrencyCheck]
        public long Version { get; set; }
    }
}
