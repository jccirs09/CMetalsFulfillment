using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMetalsFulfillment.Data.Entities;

public class MachineOperatorAssignment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BranchId { get; set; }

    [Required]
    public int MachineId { get; set; }

    [ForeignKey(nameof(MachineId))]
    public Machine Machine { get; set; } = default!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; } = default!;

    public bool IsQualified { get; set; } = true;

    public DateTime QualifiedAtUtc { get; set; } = DateTime.UtcNow;

    [Required]
    public string QualifiedByUserId { get; set; } = string.Empty;
}
