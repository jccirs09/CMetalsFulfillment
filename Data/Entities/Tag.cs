using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class Tag
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string TagNumber { get; set; } = string.Empty;

    [ConcurrencyCheck]
    public long Version { get; set; }
}
