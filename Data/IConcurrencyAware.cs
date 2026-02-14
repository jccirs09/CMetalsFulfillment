namespace CMetalsFulfillment.Data
{
    public interface IConcurrencyAware
    {
        long Version { get; set; }
    }
}
