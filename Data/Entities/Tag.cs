using CMetalsFulfillment.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities
{
    public class Tag : IConcurrencyAware
    {
        public int Id { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public string TagNumber { get; set; } = string.Empty;

        [ConcurrencyCheck]
        public long Version { get; set; }
    }
}
