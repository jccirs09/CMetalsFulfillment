namespace CMetalsFulfillment.Data.Entities;

public class MachineOperatorAssignment
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public int MachineId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime AssignedAtUtc { get; set; }
    public DateTime? UnassignedAtUtc { get; set; }

    public Machine Machine { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
