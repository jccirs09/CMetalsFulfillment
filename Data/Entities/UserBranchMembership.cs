using CMetalsFulfillment.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class UserBranchMembership
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public int BranchId { get; set; }

    [ForeignKey(nameof(UserId))]
    public ApplicationUser? User { get; set; }

    [ForeignKey(nameof(BranchId))]
    public Branch? Branch { get; set; }
}
