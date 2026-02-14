using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class ShiftTemplate
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsOvernight { get; set; }

    // Flag to support "Enable Overtime" requirement if implemented as a configuration
    public bool IsOvertime { get; set; }

    [ConcurrencyCheck]
    public long Version { get; set; }
}
