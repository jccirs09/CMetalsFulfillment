using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class RoleResolver : IRoleResolver
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    private readonly Dictionary<(string, int), HashSet<string>> _cache = new();

    public RoleResolver(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<bool> HasRole(string userId, int branchId, string roleName)
    {
        if (_cache.TryGetValue((userId, branchId), out var roles))
        {
            return roles.Contains(roleName);
        }

        using var db = await _dbFactory.CreateDbContextAsync();

        var userRoles = await db.BranchRoles
            .Where(r => r.Membership.UserId == userId && r.Membership.BranchId == branchId)
            .Select(r => r.RoleName)
            .ToListAsync();

        var roleSet = new HashSet<string>(userRoles);
        _cache[(userId, branchId)] = roleSet;

        return roleSet.Contains(roleName);
    }
}
