using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Data.Entities;

public class UserBranchMembership
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int BranchId { get; set; }
    public bool DefaultForUser { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public ICollection<UserBranchRole> Roles { get; set; } = new List<UserBranchRole>();
}
