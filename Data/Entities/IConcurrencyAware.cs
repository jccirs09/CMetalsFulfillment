using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public interface IConcurrencyAware
{
    [ConcurrencyCheck]
    long Version { get; set; }
}
