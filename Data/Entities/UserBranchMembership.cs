using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class UserBranchMembership
{
    public int BranchId { get; set; }
    public Branch Branch { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public ICollection<UserBranchRole> Roles { get; set; } = new List<UserBranchRole>();
}
