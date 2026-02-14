using CMetalsFulfillment.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities
{
    public class PickingListHeader : IConcurrencyAware
    {
        public int Id { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public string PickingListNumber { get; set; } = string.Empty;

        [ConcurrencyCheck]
        public long Version { get; set; }
    }
}
