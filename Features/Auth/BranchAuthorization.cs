using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace CMetalsFulfillment.Features.Auth
{
    public class BranchAccessRequirement : IAuthorizationRequirement { }

    public class BranchAccessHandler : AuthorizationHandler<BranchAccessRequirement>
    {
        private readonly IBranchContext _branchContext;
        private readonly IServiceProvider _serviceProvider;

        public BranchAccessHandler(IBranchContext branchContext, IServiceProvider serviceProvider)
        {
            _branchContext = branchContext;
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BranchAccessRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
                return;

            var branchId = await _branchContext.GetBranchIdAsync();
            if (branchId == 0)
            {
                // No branch selected, fail.
                context.Fail();
                return;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return;

            // Check membership
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var hasMembership = await db.UserBranchMemberships.AsNoTracking()
                .AnyAsync(m => m.UserId == userId && m.BranchId == branchId && m.IsActive);

            if (hasMembership)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }

    public class BranchRoleRequirement : IAuthorizationRequirement
    {
        public string[] AllowedRoles { get; }
        public BranchRoleRequirement(params string[] allowedRoles)
        {
            AllowedRoles = allowedRoles;
        }
    }

    public class BranchRoleHandler : AuthorizationHandler<BranchRoleRequirement>
    {
        private readonly IBranchContext _branchContext;
        private readonly IServiceProvider _serviceProvider;

        public BranchRoleHandler(IBranchContext branchContext, IServiceProvider serviceProvider)
        {
            _branchContext = branchContext;
            _serviceProvider = serviceProvider;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, BranchRoleRequirement requirement)
        {
            if (context.User.Identity?.IsAuthenticated != true)
                return;

            // SystemAdmin bypass
            // Wait, prompt says: "CanAdminBranch: SystemAdmin or BranchAdmin in BranchId".
            // Seed: "Seed a SystemAdmin user (you) and allow BranchAdmin per branch."
            // So if user is global SystemAdmin, they should pass.
            // But strict "BranchId scope" implies explicit claims.
            // However, usually SystemAdmin has access everywhere.
            // I'll check if user has global "SystemAdmin" role (Identity role).
            if (context.User.IsInRole("SystemAdmin"))
            {
                context.Succeed(requirement);
                return;
            }

            var branchId = await _branchContext.GetBranchIdAsync();
            if (branchId == 0)
            {
                context.Fail();
                return;
            }

            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return;

            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Check for any of the allowed roles for this branch
            var hasRole = await db.UserBranchClaims.AsNoTracking()
                .AnyAsync(c => c.UserId == userId && c.BranchId == branchId
                    && c.ClaimType == "role"
                    && requirement.AllowedRoles.Contains(c.ClaimValue)
                    && c.IsActive);

            if (hasRole)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
