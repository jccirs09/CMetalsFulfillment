using CMetalsFulfillment.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class UserBranchRole
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int BranchId { get; set; }
    public required string RoleName { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }
}
