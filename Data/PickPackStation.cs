using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class PickPackStation
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;

    public virtual Branch Branch { get; set; } = null!;
}
