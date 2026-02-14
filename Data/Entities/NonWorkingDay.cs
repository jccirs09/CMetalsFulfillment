namespace CMetalsFulfillment.Data.Entities;

public class NonWorkingDay
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public DateOnly Date { get; set; }
    public string Reason { get; set; } = string.Empty;
}
