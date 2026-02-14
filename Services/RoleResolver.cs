using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace CMetalsFulfillment.Services;

public class RoleResolver : IRoleResolver
{
    private readonly ApplicationDbContext _dbContext;
    // Cache roles for the current request scope: (UserId, BranchId) -> Set<RoleName>
    private readonly Dictionary<(string UserId, int BranchId), HashSet<string>> _roleCache = new();

    public RoleResolver(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasRoleAsync(string userId, int branchId, string roleName)
    {
        if (!_roleCache.TryGetValue((userId, branchId), out var roles))
        {
            var userRoles = await _dbContext.BranchRoles
                .Where(r => r.UserId == userId && r.BranchId == branchId)
                .Select(r => r.RoleName)
                .ToListAsync();

            roles = new HashSet<string>(userRoles, StringComparer.OrdinalIgnoreCase);
            _roleCache[(userId, branchId)] = roles;
        }

        return roles.Contains(roleName);
    }
}
