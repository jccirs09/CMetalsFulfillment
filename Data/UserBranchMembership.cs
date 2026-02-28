using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data
{
    public class UserBranchMembership
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        public bool IsDefaultForUser { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime AddedAtUtc { get; set; }
    }
}
