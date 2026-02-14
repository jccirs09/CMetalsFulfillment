using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities
{
    public class UserBranchRole
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BranchId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;

        public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;

        [Required]
        public string AssignedByUserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;
    }
}
