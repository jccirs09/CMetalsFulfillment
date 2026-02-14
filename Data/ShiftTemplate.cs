using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class ShiftTemplate
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsOvernight { get; set; }

    public virtual Branch Branch { get; set; } = null!;
}
