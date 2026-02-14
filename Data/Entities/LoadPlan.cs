using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class LoadPlan
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string LoadNumber { get; set; } = string.Empty;
    public string Status { get; set; } = "Planned";

    [ConcurrencyCheck]
    public long Version { get; set; }
}
