namespace CMetalsFulfillment.Data;

public class UserBranchRole
{
    public int Id { get; set; }
    public int MembershipId { get; set; }
    public required string RoleName { get; set; }

    public virtual UserBranchMembership Membership { get; set; } = null!;
}
