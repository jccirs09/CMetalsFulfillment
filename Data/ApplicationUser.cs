using CMetalsFulfillment.Domain;
using Microsoft.AspNetCore.Identity;

namespace CMetalsFulfillment.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public int? DefaultBranchId { get; set; }
        public ICollection<UserBranchMembership> BranchMemberships { get; set; } = new List<UserBranchMembership>();
        public ICollection<UserBranchClaim> BranchClaims { get; set; } = new List<UserBranchClaim>();
    }
}
