using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Data.Entities;

public class UserBranchMembership
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = default!;

    [Required]
    public int BranchId { get; set; }

    [ForeignKey(nameof(BranchId))]
    public Branch Branch { get; set; } = default!;

    public bool IsDefaultForUser { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;
}
