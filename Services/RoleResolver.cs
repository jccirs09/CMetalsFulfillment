using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace CMetalsFulfillment.Services
{
    public interface IRoleResolver
    {
        Task<bool> HasRoleAsync(string userId, int branchId, string roleName);
    }

    public class RoleResolver(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IRoleResolver
    {
        // Cache key: userId:branchId:roleName -> bool
        private readonly ConcurrentDictionary<string, bool> _cache = new();

        public async Task<bool> HasRoleAsync(string userId, int branchId, string roleName)
        {
            var key = $"{userId}:{branchId}:{roleName}";
            if (_cache.TryGetValue(key, out var hasRole))
            {
                return hasRole;
            }

            using var context = await dbContextFactory.CreateDbContextAsync();

            // Check global SystemAdmin role
            var isSystemAdmin = await context.UserRoles
                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .AnyAsync(x => x.UserId == userId && x.Name == "SystemAdmin");

            if (isSystemAdmin)
            {
                _cache[key] = true;
                return true;
            }

            if (roleName == "SystemAdmin")
            {
                 _cache[key] = false;
                 return false;
            }

            // Check branch role
            var hasBranchRole = await context.UserBranchRoles
                .AnyAsync(r => r.UserId == userId && r.BranchId == branchId && r.RoleName == roleName);

            _cache[key] = hasBranchRole;
            return hasBranchRole;
        }
    }
}
