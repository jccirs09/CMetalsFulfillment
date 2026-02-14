using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.WebUtilities;

namespace CMetalsFulfillment.Services;

public class BranchContext : IBranchContext
{
    private readonly NavigationManager _nav;
    private readonly IHttpContextAccessor _http;
    private readonly AuthenticationStateProvider _auth;
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public BranchContext(
        NavigationManager nav,
        IHttpContextAccessor http,
        AuthenticationStateProvider auth,
        IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _nav = nav;
        _http = http;
        _auth = auth;
        _dbFactory = dbFactory;
    }

    public async Task<int> GetBranchIdAsync()
    {
        var authState = await _auth.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            return 0;
        }

        var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return 0;

        using var db = await _dbFactory.CreateDbContextAsync();

        // 1. Query String
        var uri = _nav.ToAbsoluteUri(_nav.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var branchIdVal) && int.TryParse(branchIdVal, out var branchId))
        {
             // Validate membership
             var hasMembership = await db.BranchMemberships.AnyAsync(m => m.UserId == userId && m.BranchId == branchId);
             if (hasMembership) return branchId;
        }

        // 2. Cookie
        if (_http.HttpContext != null)
        {
             if (_http.HttpContext.Request.Cookies.TryGetValue("mf.branch", out var cookieBranchIdVal) && int.TryParse(cookieBranchIdVal, out var cookieBranchId))
             {
                 var hasMembership = await db.BranchMemberships.AnyAsync(m => m.UserId == userId && m.BranchId == cookieBranchId);
                 if (hasMembership) return cookieBranchId;
             }
        }

        // 3. Default for user
        var defaultBranchId = await db.Users
            .Where(u => u.Id == userId)
            .Select(u => u.DefaultBranchId)
            .FirstOrDefaultAsync();

        if (defaultBranchId.HasValue)
        {
             return defaultBranchId.Value;
        }

        return 0;
    }
}
