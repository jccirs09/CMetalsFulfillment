namespace CMetalsFulfillment.Data;

public class UserBranchMembership
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int BranchId { get; set; }
    public bool IsDefault { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
    public virtual ICollection<UserBranchRole> Roles { get; set; } = new List<UserBranchRole>();
}
