using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class RoleResolver : IRoleResolver
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    private readonly Dictionary<(string, int, string), bool> _cache = new();

    public RoleResolver(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<bool> HasRoleAsync(string userId, int branchId, string roleName)
    {
        if (_cache.TryGetValue((userId, branchId, roleName), out var hasRole))
        {
            return hasRole;
        }

        using var db = await _dbFactory.CreateDbContextAsync();
        var exists = await db.UserBranchRoles
            .AnyAsync(r => r.UserId == userId && r.BranchId == branchId && r.RoleName == roleName);

        _cache[(userId, branchId, roleName)] = exists;
        return exists;
    }
}
