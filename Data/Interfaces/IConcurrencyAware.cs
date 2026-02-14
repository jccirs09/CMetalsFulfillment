namespace CMetalsFulfillment.Data.Interfaces;

public interface IConcurrencyAware
{
    long Version { get; set; }
}
