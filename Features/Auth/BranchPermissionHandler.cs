using Microsoft.AspNetCore.Authorization;
using CMetalsFulfillment.Data;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Features.Auth
{
    public class BranchRoleRequirement : IAuthorizationRequirement
    {
        public string[] AllowedRoles { get; }

        public BranchRoleRequirement(params string[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
        }
    }

    public class BranchPermissionHandler : AuthorizationHandler<BranchRoleRequirement>
    {
        private readonly IBranchContext _branchContext;
        private readonly ApplicationDbContext _dbContext;

        public BranchPermissionHandler(IBranchContext branchContext, ApplicationDbContext dbContext)
        {
            _branchContext = branchContext;
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BranchRoleRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return;

            // SystemAdmin bypass
            // Check if user has global SystemAdmin role (via standard Identity claims)
            // IdentityRole claim type is usually ClaimTypes.Role
            if (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == AuthConstants.RoleSystemAdmin))
            {
                context.Succeed(requirement);
                return;
            }

            var branchId = _branchContext.BranchId;
            if (!branchId.HasValue)
            {
                return;
            }

            // Check if user is member of branch
            var membership = await _dbContext.UserBranchMemberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.BranchId == branchId.Value && m.IsActive);

            if (membership == null) return;

            if (requirement.AllowedRoles.Length == 0)
            {
                // Just checking for membership (BranchAccessPolicy)
                context.Succeed(requirement);
                return;
            }

            // Check if user has one of the allowed roles in this branch
            // Prompt says ClaimType="role"
            var hasRole = await _dbContext.UserBranchClaims
                .AnyAsync(c => c.UserId == userId
                            && c.BranchId == branchId.Value
                            && c.ClaimType == "role"
                            && requirement.AllowedRoles.Contains(c.ClaimValue)
                            && c.IsActive);

            if (hasRole)
            {
                context.Succeed(requirement);
            }
        }
    }
}
