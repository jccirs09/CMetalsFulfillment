using Microsoft.AspNetCore.Authorization;
using CMetalsFulfillment.Services;

namespace CMetalsFulfillment.Services.Auth
{
    public class BranchRoleRequirement(string role) : IAuthorizationRequirement
    {
        public string Role { get; } = role;
    }

    public class BranchRoleHandler(IBranchContext branchContext, IRoleResolver roleResolver) : AuthorizationHandler<BranchRoleRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BranchRoleRequirement requirement)
        {
            var userId = context.User.FindFirst("mf:userId")?.Value;
            // Also try sub or standard claim if mf:userId missing (fallback)
            if (userId == null) userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) return;

            // Check SystemAdmin claim first
            if (context.User.HasClaim(c => c.Type == "mf:isSystemAdmin"))
            {
                context.Succeed(requirement);
                return;
            }

            if (branchContext.ActiveBranchId.HasValue)
            {
                var hasRole = await roleResolver.HasRoleAsync(userId, branchContext.ActiveBranchId.Value, requirement.Role);
                if (hasRole)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
