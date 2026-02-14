using System.Security.Claims;
using CMetalsFulfillment.Services;
using Microsoft.AspNetCore.Authorization;

namespace CMetalsFulfillment.Services.Authorization;

public class BranchRoleHandler : AuthorizationHandler<BranchRoleRequirement>
{
    private readonly IBranchContext _branchContext;
    private readonly IRoleResolver _roleResolver;

    public BranchRoleHandler(IBranchContext branchContext, IRoleResolver roleResolver)
    {
        _branchContext = branchContext;
        _roleResolver = roleResolver;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BranchRoleRequirement requirement)
    {
        // SystemAdmin always passes
        if (context.User.IsInRole("SystemAdmin") || context.User.HasClaim(c => c.Type == "mf:isSystemAdmin"))
        {
            context.Succeed(requirement);
            return;
        }

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return;

        var branchId = await _branchContext.GetBranchIdAsync();
        if (branchId == 0) return;

        foreach (var role in requirement.AllowedRoles)
        {
            if (await _roleResolver.HasRoleAsync(userId, branchId, role))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
