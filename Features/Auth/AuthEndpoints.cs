using System.Security.Claims;
using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;

namespace CMetalsFulfillment.Features.Auth
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/me").RequireAuthorization();

            group.MapGet("/branches", async (ApplicationDbContext db, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Results.Unauthorized();

                var memberships = await db.UserBranchMemberships
                    .Include(m => m.Branch)
                    .Where(m => m.UserId == userId && m.IsActive)
                    .Select(m => new { m.BranchId, m.Branch.Name, m.DefaultForUser })
                    .ToListAsync();

                return Results.Ok(memberships);
            });
        }
    }
}
