using System.Security.Claims;
using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class BranchClaimsTransformation : IClaimsTransformation
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public BranchClaimsTransformation(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return principal;
        }

        var clone = principal.Clone();
        var newIdentity = (ClaimsIdentity)clone.Identity!;

        var userId = newIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return principal;

        if (!clone.HasClaim(c => c.Type == "mf:userId"))
        {
            newIdentity.AddClaim(new Claim("mf:userId", userId));
        }

        using var db = await _dbFactory.CreateDbContextAsync();

        // Check if SystemAdmin
        // We need to access base Identity tables.
        // IdentityDbContext exposes Roles and UserRoles.
        var isSystemAdmin = await db.UserRoles
            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
            .AnyAsync(x => x.UserId == userId && x.Name == "SystemAdmin");

        if (isSystemAdmin)
        {
             if (!clone.HasClaim(c => c.Type == "mf:isSystemAdmin"))
             {
                 newIdentity.AddClaim(new Claim("mf:isSystemAdmin", "true"));
             }
             if (!clone.IsInRole("SystemAdmin"))
             {
                 newIdentity.AddClaim(new Claim(newIdentity.RoleClaimType, "SystemAdmin"));
             }
        }

        if (!clone.HasClaim(c => c.Type == "mf:defaultBranchId"))
        {
            var membership = await db.UserBranchMemberships
                .Where(m => m.UserId == userId && m.IsDefaultForUser)
                .Select(m => m.BranchId)
                .FirstOrDefaultAsync();

            if (membership != 0)
            {
                newIdentity.AddClaim(new Claim("mf:defaultBranchId", membership.ToString()));
            }
        }

        return clone;
    }
}
