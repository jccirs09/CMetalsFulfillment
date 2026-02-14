using CMetalsFulfillment.Data;

namespace CMetalsFulfillment.Services.Interfaces;

public interface IUserAdminService
{
    Task<List<ApplicationUser>> SearchUsersAsync(string? query);
    Task<List<ApplicationUser>> GetUsersByBranchAsync(int branchId);
    Task SetBranchMembershipAsync(string userId, int branchId, bool isActive, bool isDefault);
    Task SetBranchRoleAsync(string userId, int branchId, string roleName, bool isActive);
}
