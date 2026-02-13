using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Domain
{
    public class UserBranchMembership
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool DefaultForUser { get; set; }

        [ForeignKey("BranchId")]
        public Branch? Branch { get; set; }
    }
}
