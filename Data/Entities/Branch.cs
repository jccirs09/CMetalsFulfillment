namespace CMetalsFulfillment.Data.Entities;

public class Branch
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string TimezoneId { get; set; }
}
