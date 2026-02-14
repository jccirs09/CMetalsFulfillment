using CMetalsFulfillment.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CMetalsFulfillment.Services
{
    public class BranchRoleRequirement(params string[] roleNames) : IAuthorizationRequirement
    {
        public string[] RoleNames { get; } = roleNames;
    }

    public class BranchAuthorizationHandler(
        IBranchContext branchContext,
        IRoleResolver roleResolver) : AuthorizationHandler<BranchRoleRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BranchRoleRequirement requirement)
        {
            var user = context.User;
            if (user == null || !user.Identity.IsAuthenticated) return;

            // SystemAdmin bypass (Global role)
            if (user.IsInRole("SystemAdmin") || user.HasClaim(c => c.Type == "mf:isSystemAdmin"))
            {
                context.Succeed(requirement);
                return;
            }

            var userId = user.FindFirst("mf:userId")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            if (branchContext.ActiveBranchId.HasValue)
            {
                foreach (var role in requirement.RoleNames)
                {
                    if (await roleResolver.HasRoleAsync(userId, branchContext.ActiveBranchId.Value, role))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
            }
        }
    }
}
