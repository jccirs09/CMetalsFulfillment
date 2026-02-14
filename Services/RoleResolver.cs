using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public interface IRoleResolver
    {
        Task<bool> HasRoleAsync(string userId, int branchId, string roleName);
    }

    public class RoleResolver(IDbContextFactory<ApplicationDbContext> dbFactory) : IRoleResolver
    {
        private readonly Dictionary<(string UserId, int BranchId, string RoleName), bool> _cache = [];

        public async Task<bool> HasRoleAsync(string userId, int branchId, string roleName)
        {
            var key = (userId, branchId, roleName);
            if (_cache.TryGetValue(key, out var cachedResult))
            {
                return cachedResult;
            }

            using var context = await dbFactory.CreateDbContextAsync();
            var hasRole = await context.UserBranchRoles
                .AnyAsync(r => r.UserId == userId && r.BranchId == branchId && r.RoleName == roleName);

            _cache[key] = hasRole;
            return hasRole;
        }
    }
}
