using CMetalsFulfillment.Domain.Entities;

namespace CMetalsFulfillment.Services.Interfaces;

public interface IBranchService
{
    Task<List<Branch>> GetAllBranchesAsync();
    Task<Branch?> GetBranchByIdAsync(int id);
    Task<Branch> CreateBranchAsync(Branch branch);
    Task UpdateBranchAsync(Branch branch);
    Task DeleteBranchAsync(int id);
}
