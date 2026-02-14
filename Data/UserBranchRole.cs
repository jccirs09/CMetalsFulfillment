using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data
{
    public class UserBranchRole
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = default!;

        [Required]
        public int BranchId { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch Branch { get; set; } = default!;

        [Required]
        public string RoleName { get; set; } = string.Empty;

        public DateTime AssignedAtUtc { get; set; }

        [Required]
        public string AssignedByUserId { get; set; } = string.Empty;
    }
}
