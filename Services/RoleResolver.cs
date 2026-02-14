using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class RoleResolver : IRoleResolver
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public RoleResolver(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<bool> HasRoleAsync(string userId, int branchId, string role)
    {
        using var context = await _dbFactory.CreateDbContextAsync();
        var roles = await context.BranchRoles
            .Where(r => r.UserId == userId && r.BranchId == branchId)
            .Select(r => r.RoleName)
            .AsNoTracking()
            .ToListAsync();

        return roles.Contains(role);
    }
}
