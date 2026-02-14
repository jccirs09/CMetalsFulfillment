using System.Security.Claims;
using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services.Auth
{
    public class BranchClaimsTransformation(IServiceProvider serviceProvider) : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity is not ClaimsIdentity identity || !identity.IsAuthenticated)
            {
                return principal;
            }

            // Avoid recursion or re-adding
            if (principal.HasClaim(c => c.Type == "mf:userId")) return principal;

            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

            var user = await userManager.GetUserAsync(principal);
            if (user == null) return principal;

            identity.AddClaim(new Claim("mf:userId", user.Id));

            // Check SystemAdmin
            using var context = await contextFactory.CreateDbContextAsync();
            var isSystemAdmin = await context.UserRoles
                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .AnyAsync(x => x.UserId == user.Id && x.Name == "SystemAdmin");

            if (isSystemAdmin)
            {
                identity.AddClaim(new Claim("mf:isSystemAdmin", "true"));
            }

            // Default Branch
            var defaultMembership = await context.UserBranchMemberships
                .FirstOrDefaultAsync(m => m.UserId == user.Id && m.IsDefaultForUser && m.IsActive);

            if (defaultMembership != null)
            {
                 identity.AddClaim(new Claim("mf:defaultBranchId", defaultMembership.BranchId.ToString()));
            }

            return principal;
        }
    }
}
