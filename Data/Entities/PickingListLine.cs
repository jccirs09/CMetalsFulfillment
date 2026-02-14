using CMetalsFulfillment.Data.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities
{
    public class PickingListLine : IConcurrencyAware
    {
        public int Id { get; set; }
        [Required]
        public int PickingListHeaderId { get; set; }
        public int LineNumber { get; set; }

        [ConcurrencyCheck]
        public long Version { get; set; }

        public string ReservedMaterialsJson { get; set; } = "[]";
    }
}
