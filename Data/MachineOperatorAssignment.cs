using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data;

public class MachineOperatorAssignment
{
    public int Id { get; set; }
    public int MachineId { get; set; }
    public required string UserId { get; set; }
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    public virtual Machine Machine { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}
