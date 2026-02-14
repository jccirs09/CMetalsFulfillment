using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CMetalsFulfillment.Services;

public class BranchClaimsTransformation(UserManager<ApplicationUser> userManager) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
        {
            return principal;
        }

        // Avoid re-adding claims if already present
        if (identity.HasClaim(c => c.Type == "mf:userId"))
        {
            return principal;
        }

        var user = await userManager.GetUserAsync(principal);
        if (user != null)
        {
            identity.AddClaim(new Claim("mf:userId", user.Id));

            if (user.DefaultBranchId.HasValue)
            {
                identity.AddClaim(new Claim("mf:defaultBranchId", user.DefaultBranchId.Value.ToString()));
            }

            // Check if SystemAdmin (Global Role)
            if (await userManager.IsInRoleAsync(user, "SystemAdmin"))
            {
                identity.AddClaim(new Claim("mf:isSystemAdmin", "true"));
            }
        }

        return principal;
    }
}
