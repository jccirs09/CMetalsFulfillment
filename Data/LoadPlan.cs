using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data
{
    public class LoadPlan : IConcurrencyAware
    {
        public int Id { get; set; }

        [ConcurrencyCheck]
        public long Version { get; set; }
    }
}
