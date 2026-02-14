using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class Machine
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MachineType { get; set; } = "CTL"; // CTL, Slitter, etc.

    public virtual Branch Branch { get; set; } = null!;
}
