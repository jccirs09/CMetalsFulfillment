namespace CMetalsFulfillment.Data;

public class Branch
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string TimezoneId { get; set; } = "UTC";
}
