using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data
{
    public interface IConcurrencyAware
    {
        [ConcurrencyCheck]
        long Version { get; set; }
    }
}
