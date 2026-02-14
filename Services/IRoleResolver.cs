namespace CMetalsFulfillment.Services;

public interface IRoleResolver
{
    Task<bool> HasRoleAsync(string userId, int branchId, string role);
}
