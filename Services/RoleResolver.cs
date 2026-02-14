using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace CMetalsFulfillment.Services;

public class RoleResolver(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IRoleResolver
{
    private readonly ConcurrentDictionary<(string, int), HashSet<string>> _roleCache = new();

    public async Task<bool> HasRoleAsync(string userId, int branchId, string roleName)
    {
        if (string.IsNullOrEmpty(userId) || branchId <= 0)
            return false;

        // Check Cache
        if (_roleCache.TryGetValue((userId, branchId), out var roles))
        {
            return roles.Contains(roleName);
        }

        // Fetch from DB
        using var context = await dbContextFactory.CreateDbContextAsync();

        var userRoles = await context.UserBranchRoles
            .Where(r => r.Membership.UserId == userId && r.Membership.BranchId == branchId)
            .Select(r => r.RoleName)
            .ToListAsync();

        var roleSet = new HashSet<string>(userRoles, StringComparer.OrdinalIgnoreCase);
        _roleCache.TryAdd((userId, branchId), roleSet);

        return roleSet.Contains(roleName);
    }
}
