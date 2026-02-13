using System.Security.Claims;
using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using CMetalsFulfillment.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Features.Admin
{
    public static class AdminEndpoints
    {
        public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/admin/users").RequireAuthorization();

            // Assign branch membership
            group.MapPost("/{userId}/branches", async (
                string userId,
                [FromBody] AssignBranchRequest request,
                ApplicationDbContext db,
                IBranchContext branchContext,
                ClaimsPrincipal user) =>
            {
                // Must be SystemAdmin OR Admin of the target branch
                bool isSystemAdmin = user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == AuthConstants.RoleSystemAdmin);

                if (!isSystemAdmin)
                {
                     // Check if current context branch matches request branch and user is admin of it
                     if (branchContext.BranchId != request.BranchId)
                     {
                         return Results.Forbid();
                     }

                     // Check if user is BranchAdmin of this branch
                     var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                     var isAdmin = await db.UserBranchClaims.AnyAsync(c => c.UserId == currentUserId && c.BranchId == request.BranchId && c.ClaimType == "role" && c.ClaimValue == AuthConstants.RoleBranchAdmin && c.IsActive);
                     if (!isAdmin) return Results.Forbid();
                }

                var membership = await db.UserBranchMemberships.FirstOrDefaultAsync(m => m.UserId == userId && m.BranchId == request.BranchId);
                if (membership == null)
                {
                    membership = new UserBranchMembership { UserId = userId, BranchId = request.BranchId, IsActive = true, DefaultForUser = request.DefaultForUser };
                    db.UserBranchMemberships.Add(membership);
                }
                else
                {
                    membership.IsActive = request.IsActive;
                    membership.DefaultForUser = request.DefaultForUser;
                }

                // If set as default, clear other defaults
                if (request.DefaultForUser)
                {
                    var others = await db.UserBranchMemberships.Where(m => m.UserId == userId && m.BranchId != request.BranchId && m.DefaultForUser).ToListAsync();
                    foreach(var o in others) o.DefaultForUser = false;
                }

                await db.SaveChangesAsync();
                return Results.Ok();
            });

            // Assign branch claims
            group.MapPost("/{userId}/claims", async (
                string userId,
                [FromBody] AssignClaimRequest request,
                ApplicationDbContext db,
                IBranchContext branchContext,
                ClaimsPrincipal user) =>
            {
                bool isSystemAdmin = user.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == AuthConstants.RoleSystemAdmin);
                 if (!isSystemAdmin)
                {
                     if (branchContext.BranchId != request.BranchId) return Results.Forbid();
                     var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                     var isAdmin = await db.UserBranchClaims.AnyAsync(c => c.UserId == currentUserId && c.BranchId == request.BranchId && c.ClaimType == "role" && c.ClaimValue == AuthConstants.RoleBranchAdmin && c.IsActive);
                     if (!isAdmin) return Results.Forbid();
                }

                var claim = await db.UserBranchClaims.FirstOrDefaultAsync(c => c.UserId == userId && c.BranchId == request.BranchId && c.ClaimType == request.ClaimType && c.ClaimValue == request.ClaimValue);
                if (claim == null)
                {
                    if (request.IsActive)
                    {
                        db.UserBranchClaims.Add(new UserBranchClaim { UserId = userId, BranchId = request.BranchId, ClaimType = request.ClaimType, ClaimValue = request.ClaimValue, IsActive = true });
                    }
                }
                else
                {
                    claim.IsActive = request.IsActive;
                }

                await db.SaveChangesAsync();
                return Results.Ok();
            });
        }
    }

    public class AssignBranchRequest
    {
        public int BranchId { get; set; }
        public bool IsActive { get; set; }
        public bool DefaultForUser { get; set; }
    }

    public class AssignClaimRequest
    {
        public int BranchId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
        public bool IsActive { get; set; }
    }
}
