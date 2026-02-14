using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Data.Entities;

public class WorkOrder
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = "Created";

    [ConcurrencyCheck]
    public long Version { get; set; }
}
