using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using CMetalsFulfillment.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMetalsFulfillment.Features.Admin
{
    public static class AdminEndpoints
    {
        public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/admin").RequireAuthorization();

            group.MapPost("/users/{userId}/branches", async (
                string userId,
                [FromBody] AssignBranchDto dto,
                ApplicationDbContext db,
                ClaimsPrincipal user,
                IBranchContext branchContext) =>
            {
                // Check permissions
                // If user is SystemAdmin, allow.
                // If adding to a branch, requestor must be BranchAdmin of that branch.
                // Here, we can't easily rely on BranchContext if the request body specifies the branch.
                // So we manually check.

                bool isSystemAdmin = user.IsInRole("SystemAdmin");
                if (!isSystemAdmin)
                {
                    // Check if user is BranchAdmin for the target branch
                    var requestorId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                    var hasAdmin = await db.UserBranchClaims
                        .AnyAsync(c => c.UserId == requestorId && c.BranchId == dto.BranchId
                            && c.ClaimType == "role" && c.ClaimValue == "BranchAdmin" && c.IsActive);

                    if (!hasAdmin) return Results.Forbid();
                }

                var membership = await db.UserBranchMemberships
                    .FirstOrDefaultAsync(m => m.UserId == userId && m.BranchId == dto.BranchId);

                if (membership == null)
                {
                    membership = new UserBranchMembership
                    {
                        UserId = userId,
                        BranchId = dto.BranchId,
                        IsActive = dto.IsActive,
                        DefaultForUser = dto.DefaultForUser
                    };
                    db.UserBranchMemberships.Add(membership);
                }
                else
                {
                    membership.IsActive = dto.IsActive;
                    membership.DefaultForUser = dto.DefaultForUser;
                }

                if (dto.DefaultForUser)
                {
                    // Unset other defaults for this user
                    var others = await db.UserBranchMemberships
                        .Where(m => m.UserId == userId && m.BranchId != dto.BranchId && m.DefaultForUser)
                        .ToListAsync();
                    foreach (var m in others) m.DefaultForUser = false;

                    // Update user DefaultBranchId
                    var userEntity = await db.Users.FindAsync(userId);
                    if (userEntity != null)
                    {
                        userEntity.DefaultBranchId = dto.BranchId;
                    }
                }

                await db.SaveChangesAsync();
                return Results.Ok();
            });

            group.MapPost("/users/{userId}/claims", async (
                string userId,
                [FromBody] AssignClaimDto dto,
                ApplicationDbContext db,
                ClaimsPrincipal user) =>
            {
                bool isSystemAdmin = user.IsInRole("SystemAdmin");
                if (!isSystemAdmin)
                {
                    var requestorId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                    var hasAdmin = await db.UserBranchClaims
                        .AnyAsync(c => c.UserId == requestorId && c.BranchId == dto.BranchId
                            && c.ClaimType == "role" && c.ClaimValue == "BranchAdmin" && c.IsActive);

                    if (!hasAdmin) return Results.Forbid();
                }

                var existing = await db.UserBranchClaims
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BranchId == dto.BranchId
                        && c.ClaimType == dto.ClaimType && c.ClaimValue == dto.ClaimValue);

                if (existing == null)
                {
                    // If creating new claim, maybe deactivate others if logic requires?
                    // Prompt says "Role changes... append-only audit".
                    // But here we are just setting claims.
                    // For "role", typically a user has one role per branch? Or multiple?
                    // Prompt: "ClaimType='role', ClaimValue".
                    // We'll allow multiple for now, or just append.

                    var claim = new UserBranchClaim
                    {
                        UserId = userId,
                        BranchId = dto.BranchId,
                        ClaimType = dto.ClaimType,
                        ClaimValue = dto.ClaimValue,
                        IsActive = true
                    };
                    db.UserBranchClaims.Add(claim);
                }
                else
                {
                    existing.IsActive = true; // Reactivate if exists
                }

                await db.SaveChangesAsync();
                return Results.Ok();
            });

            group.MapGet("/setup/status", async (SetupStatusService setupService) =>
            {
                var status = await setupService.GetStatusAsync();
                return Results.Ok(status);
            });
        }
    }

    public class AssignBranchDto
    {
        public int BranchId { get; set; }
        public bool IsActive { get; set; }
        public bool DefaultForUser { get; set; }
    }

    public class AssignClaimDto
    {
        public int BranchId { get; set; }
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
    }
}
