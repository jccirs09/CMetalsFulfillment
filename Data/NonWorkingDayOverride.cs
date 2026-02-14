using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class NonWorkingDayOverride
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public DateOnly Date { get; set; }
    [Required]
    public string Reason { get; set; } = string.Empty;
    public required string ApprovedByUserId { get; set; }

    public virtual Branch Branch { get; set; } = null!;
    public virtual ApplicationUser ApprovedByUser { get; set; } = null!;
}
