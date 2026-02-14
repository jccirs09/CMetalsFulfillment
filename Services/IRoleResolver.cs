namespace CMetalsFulfillment.Services;

public interface IRoleResolver
{
    Task<bool> HasRole(string userId, int branchId, string roleName);
}
