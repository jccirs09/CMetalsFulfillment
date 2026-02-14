using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services;

public interface IBranchContext
{
    Task<int> GetBranchIdAsync();
    Task<Branch?> GetBranchAsync();
    Task SetBranchAsync(int branchId);
}
