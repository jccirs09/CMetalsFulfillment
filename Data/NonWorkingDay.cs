using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class NonWorkingDay
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public DateOnly Date { get; set; }
    public string Description { get; set; } = string.Empty;

    public virtual Branch Branch { get; set; } = null!;
}
