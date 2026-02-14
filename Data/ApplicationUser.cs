using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Data;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAtUtc { get; set; }

    // Navigation properties for relationships
    public ICollection<UserBranchMembership> BranchMemberships { get; set; } = new List<UserBranchMembership>();
    public ICollection<UserBranchRole> BranchRoles { get; set; } = new List<UserBranchRole>();
}
