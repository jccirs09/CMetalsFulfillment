using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMetalsFulfillment.Features.Auth
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/me").RequireAuthorization();

            group.MapGet("/branches", async (ApplicationDbContext db, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

                var memberships = await db.UserBranchMemberships
                    .Where(m => m.UserId == userId && m.IsActive)
                    .Include(m => m.Branch)
                    .Select(m => new
                    {
                        m.BranchId,
                        BranchName = m.Branch != null ? m.Branch.Name : "",
                        m.DefaultForUser
                    })
                    .ToListAsync();

                return Results.Ok(memberships);
            });
        }
    }
}
