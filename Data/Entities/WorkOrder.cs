using CMetalsFulfillment.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities
{
    public class WorkOrder : IConcurrencyAware
    {
        public int Id { get; set; }
        [Required]
        public int BranchId { get; set; }
        [Required]
        public string OrderNumber { get; set; } = string.Empty;

        [ConcurrencyCheck]
        public long Version { get; set; }
    }
}
