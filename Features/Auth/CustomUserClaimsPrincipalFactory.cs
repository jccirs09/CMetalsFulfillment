using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Features.Auth
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly ApplicationDbContext _db;

        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> options,
            ApplicationDbContext db)
            : base(userManager, roleManager, options)
        {
            _db = db;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Add branch-specific roles for the default branch
            if (user.DefaultBranchId.HasValue)
            {
                var roleClaims = await _db.UserBranchClaims
                    .Where(c => c.UserId == user.Id && c.BranchId == user.DefaultBranchId.Value
                        && c.ClaimType == "role" && c.IsActive)
                    .Select(c => c.ClaimValue)
                    .ToListAsync();

                foreach (var role in roleClaims)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                // Also add BranchId claim for convenience if needed
                identity.AddClaim(new Claim("DefaultBranchId", user.DefaultBranchId.Value.ToString()));
            }

            return identity;
        }
    }
}
