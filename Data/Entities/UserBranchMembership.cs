using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities
{
    public class UserBranchMembership
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BranchId { get; set; }

        public bool IsDefaultForUser { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; } = null!;
    }
}
