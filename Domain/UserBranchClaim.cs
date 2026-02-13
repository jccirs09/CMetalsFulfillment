using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class UserBranchClaim
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public int BranchId { get; set; }
        [Required]
        public string ClaimType { get; set; } = string.Empty;
        [Required]
        public string ClaimValue { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }
    }
}
