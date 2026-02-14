using Microsoft.AspNetCore.Identity;
using CMetalsFulfillment.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public int? DefaultBranchId { get; set; }

    [ForeignKey(nameof(DefaultBranchId))]
    public Branch? DefaultBranch { get; set; }

    public ICollection<UserBranchMembership> BranchMemberships { get; set; } = new List<UserBranchMembership>();
    public ICollection<UserBranchRole> BranchRoles { get; set; } = new List<UserBranchRole>();
}
