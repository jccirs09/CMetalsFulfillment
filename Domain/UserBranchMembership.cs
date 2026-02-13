using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Domain
{
    public class UserBranchMembership
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int BranchId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool DefaultForUser { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [ForeignKey(nameof(BranchId))]
        public Branch? Branch { get; set; }
    }
}
