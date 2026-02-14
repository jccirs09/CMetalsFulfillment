using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class UserBranchRole
{
    public int Id { get; set; }

    public int BranchId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public UserBranchMembership Membership { get; set; } = null!;

    [Required]
    public string RoleName { get; set; } = string.Empty;
}
