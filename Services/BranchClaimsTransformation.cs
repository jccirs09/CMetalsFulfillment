using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CMetalsFulfillment.Services
{
    public class BranchClaimsTransformation(
        UserManager<ApplicationUser> userManager,
        IUserBranchService userBranchService) : IClaimsTransformation
    {
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = principal.Identity as ClaimsIdentity;
            if (identity == null || !identity.IsAuthenticated)
            {
                return principal;
            }

            // Avoid loop or duplicate work
            if (principal.HasClaim(c => c.Type == "mf:transformed"))
            {
                return principal;
            }

            var userId = userManager.GetUserId(principal);
            if (string.IsNullOrEmpty(userId)) return principal;

            // Clone the identity to avoid side effects on the original principal if cached elsewhere?
            // Standard practice: create a new principal with the augmented identity, or add to existing.
            // Identity documentation says TransformAsync should return a NEW principal.

            var newIdentity = new ClaimsIdentity(identity.Claims, identity.AuthenticationType, identity.NameClaimType, identity.RoleClaimType);

            // mf:userId
            if (!newIdentity.HasClaim(c => c.Type == "mf:userId"))
            {
                newIdentity.AddClaim(new Claim("mf:userId", userId));
            }

            // mf:isSystemAdmin
            // Use UserManager to check role.
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                if (await userManager.IsInRoleAsync(user, "SystemAdmin"))
                {
                    if (!newIdentity.HasClaim(c => c.Type == "mf:isSystemAdmin"))
                    {
                        newIdentity.AddClaim(new Claim("mf:isSystemAdmin", "true"));
                    }
                }
            }

            // mf:defaultBranchId
            var defaultMembership = await userBranchService.GetDefaultMembershipAsync(userId);
            if (defaultMembership != null)
            {
                if (!newIdentity.HasClaim(c => c.Type == "mf:defaultBranchId"))
                {
                    newIdentity.AddClaim(new Claim("mf:defaultBranchId", defaultMembership.BranchId.ToString()));
                }
            }

            newIdentity.AddClaim(new Claim("mf:transformed", "true"));

            return new ClaimsPrincipal(newIdentity);
        }
    }
}
