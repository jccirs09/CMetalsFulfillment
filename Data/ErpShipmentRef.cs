using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data
{
    public class ErpShipmentRef : IConcurrencyAware
    {
        public int Id { get; set; }

        [ConcurrencyCheck]
        public long Version { get; set; }
    }
}
