using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class Truck
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public decimal MaxWeightLbs { get; set; }

    public virtual Branch Branch { get; set; } = null!;
}
