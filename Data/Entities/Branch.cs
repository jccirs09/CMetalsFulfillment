using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class Branch
{
    public int BranchId { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Code { get; set; } = string.Empty;

    public BranchSettings Settings { get; set; } = new();

    public ICollection<UserBranchMembership> Memberships { get; set; } = new List<UserBranchMembership>();
}
