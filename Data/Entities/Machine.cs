using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class Machine
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";

    [ConcurrencyCheck]
    public long Version { get; set; }
}
