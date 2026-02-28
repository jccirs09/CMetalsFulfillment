using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data
{
    public class UserBranchRole
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        public DateTime AssignedAtUtc { get; set; }

        [Required]
        public string AssignedByUserId { get; set; } = string.Empty;
    }
}
