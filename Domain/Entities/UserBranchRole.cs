using System.ComponentModel.DataAnnotations;
using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Domain.Entities;

public class UserBranchRole
{
    public int Id { get; set; }
    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int BranchId { get; set; }
    public Branch? Branch { get; set; }

    [Required]
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
