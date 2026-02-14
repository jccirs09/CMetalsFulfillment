using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class ErpShipmentRef
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string ErpPackingListNumber { get; set; } = string.Empty;

    [ConcurrencyCheck]
    public long Version { get; set; }
}
