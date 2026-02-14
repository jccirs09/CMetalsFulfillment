namespace CMetalsFulfillment.Services;

public interface IBranchContext
{
    Task<int> GetBranchIdAsync();
}
