using Microsoft.AspNetCore.Identity;

namespace CMetalsFulfillment.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? LastLoginAtUtc { get; set; }

        public int? DefaultBranchId { get; set; }
        public ICollection<UserBranchMembership> Memberships { get; set; } = new List<UserBranchMembership>();
    }
}
