using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CMetalsFulfillment.Services;

public class BranchClaimsTransformation : IClaimsTransformation
{
    private readonly UserManager<ApplicationUser> _userManager;

    public BranchClaimsTransformation(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identity as ClaimsIdentity;
        if (identity == null || !identity.IsAuthenticated)
        {
            return principal;
        }

        // Check if we already transformed to avoid duplicates
        if (identity.HasClaim(c => c.Type == "mf:userId"))
            return principal;

        var user = await _userManager.GetUserAsync(principal);
        if (user == null)
        {
            return principal;
        }

        // Clone the identity to avoid side effects on the original principal if cached elsewhere?
        // Actually, in IClaimsTransformation, we typically return the same principal modified or a new one.
        // Modifying the existing one is common if it's not shared.

        var newIdentity = new ClaimsIdentity(identity.Claims, identity.AuthenticationType, identity.NameClaimType, identity.RoleClaimType);

        newIdentity.AddClaim(new Claim("mf:userId", user.Id));

        if (user.DefaultBranchId.HasValue)
        {
            newIdentity.AddClaim(new Claim("mf:defaultBranchId", user.DefaultBranchId.Value.ToString()));
        }

        // Check for SystemAdmin role
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Contains("SystemAdmin"))
        {
            newIdentity.AddClaim(new Claim("mf:isSystemAdmin", "true"));
        }

        return new ClaimsPrincipal(newIdentity);
    }
}
