namespace CMetalsFulfillment.Data.Entities;

public class UserBranchRole
{
    public int Id { get; set; }
    public int UserBranchMembershipId { get; set; }
    public string RoleName { get; set; } = string.Empty;

    public UserBranchMembership Membership { get; set; } = null!;
}
