namespace CMetalsFulfillment.Data.Entities;

public class NonWorkingDayOverride
{
    public int Id { get; set; }
    public int BranchId { get; set; }
    public DateOnly Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsWorkingDay { get; set; }
}
