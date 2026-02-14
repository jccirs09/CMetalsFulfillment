namespace CMetalsFulfillment.Services.Interfaces;

public interface IBranchContext
{
    Task<int> GetBranchIdAsync();
    Task SetBranchIdAsync(int branchId);
}
