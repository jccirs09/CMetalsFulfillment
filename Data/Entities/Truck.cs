using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class Truck
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string LicensePlate { get; set; } = string.Empty;
    public int MaxWeightLbs { get; set; }
    public string Status { get; set; } = "Active";

    [ConcurrencyCheck]
    public long Version { get; set; }
}
