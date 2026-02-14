namespace CMetalsFulfillment.Data.Entities;

public class Branch
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TimezoneId { get; set; } = "UTC";
}
