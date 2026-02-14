using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CMetalsFulfillment.Services;

public class BranchContext : IBranchContext
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    private readonly NavigationManager _navigationManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly IJSRuntime _jsRuntime;

    private int? _branchId;
    private Branch? _branch;

    public BranchContext(
        IDbContextFactory<ApplicationDbContext> dbFactory,
        NavigationManager navigationManager,
        IHttpContextAccessor httpContextAccessor,
        AuthenticationStateProvider authenticationStateProvider,
        IJSRuntime jsRuntime)
    {
        _dbFactory = dbFactory;
        _navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
        _authenticationStateProvider = authenticationStateProvider;
        _jsRuntime = jsRuntime;
    }

    public async Task<int> GetBranchIdAsync()
    {
        if (_branchId.HasValue) return _branchId.Value;

        // 1. Check Query String
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        if (Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var branchIdStr))
        {
            if (int.TryParse(branchIdStr, out var bId))
            {
                _branchId = bId;
                return bId;
            }
        }

        // 2. Check Cookie (Initial Load via HttpContext)
        if (_httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue("mf.branch", out var cookieBranchIdStr) == true)
        {
             if (int.TryParse(cookieBranchIdStr, out var cId))
            {
                _branchId = cId;
                return cId;
            }
        }

        // 3. Check User Default
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity?.IsAuthenticated == true)
        {
             // Check claim first
             var claim = user.FindFirst("mf:defaultBranchId");
             if (claim != null && int.TryParse(claim.Value, out var claimId))
             {
                 _branchId = claimId;
                 return claimId;
             }

             // Fallback to DB
             using var db = await _dbFactory.CreateDbContextAsync();
             var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
             if (!string.IsNullOrEmpty(userId))
             {
                 var membership = await db.UserBranchMemberships
                     .Where(m => m.UserId == userId && m.IsDefaultForUser)
                     .FirstOrDefaultAsync();
                 if (membership != null)
                 {
                     _branchId = membership.BranchId;
                     return membership.BranchId;
                 }
             }
        }

        return 0;
    }

    public async Task<Branch?> GetBranchAsync()
    {
        if (_branch != null) return _branch;

        var branchId = await GetBranchIdAsync();
        if (branchId == 0) return null;

        using var db = await _dbFactory.CreateDbContextAsync();
        _branch = await db.Branches.FindAsync(branchId);
        return _branch;
    }

    public async Task SetBranchAsync(int branchId)
    {
        _branchId = branchId;
        _branch = null;

        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'mf.branch={branchId}; path=/; max-age=31536000;'");
        }
        catch (Exception) { }
    }
}
