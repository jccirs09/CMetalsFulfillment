using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace CMetalsFulfillment.Services;

public class BranchContext : IBranchContext
{
    private readonly NavigationManager _navigationManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ApplicationDbContext _dbContext;
    private readonly IJSRuntime _jsRuntime;

    private int? _cachedBranchId;

    public BranchContext(
        NavigationManager navigationManager,
        IHttpContextAccessor httpContextAccessor,
        AuthenticationStateProvider authenticationStateProvider,
        ApplicationDbContext dbContext,
        IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
        _authenticationStateProvider = authenticationStateProvider;
        _dbContext = dbContext;
        _jsRuntime = jsRuntime;
    }

    public async Task<int?> GetBranchIdAsync()
    {
        if (_cachedBranchId.HasValue)
            return _cachedBranchId.Value;

        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (!user.Identity?.IsAuthenticated ?? true)
            return null;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return null;

        // 1. Query String
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        if (Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var branchIdVal)
            && int.TryParse(branchIdVal, out var branchId))
        {
             if (await ValidateMembership(userId, branchId))
             {
                 _cachedBranchId = branchId;
                 return branchId;
             }
        }

        // 2. Cookie (mf.branch)
        int? cookieBranchId = null;
        if (_httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue("mf.branch", out var cookieVal) == true)
        {
             if (int.TryParse(cookieVal, out var val)) cookieBranchId = val;
        }
        else
        {
             try
             {
                // Fallback to JS if HttpContext is not available (circuit)
                var cookieString = await _jsRuntime.InvokeAsync<string>("eval", "document.cookie");
                var cookies = cookieString.Split(';');
                foreach (var c in cookies)
                {
                    var parts = c.Trim().Split('=');
                    if (parts.Length == 2 && parts[0] == "mf.branch")
                    {
                        if (int.TryParse(parts[1], out var val)) cookieBranchId = val;
                        break;
                    }
                }
             }
             catch { /* JS might not be available during prerender or if disconnected */ }
        }

        if (cookieBranchId.HasValue)
        {
             if (await ValidateMembership(userId, cookieBranchId.Value))
             {
                 _cachedBranchId = cookieBranchId.Value;
                 return cookieBranchId.Value;
             }
        }

        // 3. DefaultForUser
        // We need to fetch the user again or check claims if we added it there (but we haven't implemented claims transformation yet completely or logic relies on DB)
        // Since we are in BranchContext, we can query DB.
        var userEntity = await _dbContext.Users.FindAsync(userId);
        if (userEntity?.DefaultBranchId != null)
        {
             _cachedBranchId = userEntity.DefaultBranchId;
             return _cachedBranchId;
        }

        return null;
    }

    public async Task SetBranchIdAsync(int branchId)
    {
         // Set cookie via JS Interop
         await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'mf.branch={branchId}; path=/; max-age=31536000;'");
         _cachedBranchId = branchId;
    }

    private async Task<bool> ValidateMembership(string userId, int branchId)
    {
        return await _dbContext.BranchMemberships.AnyAsync(m => m.UserId == userId && m.BranchId == branchId);
    }
}
