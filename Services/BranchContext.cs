using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using MudBlazor;

namespace CMetalsFulfillment.Services;

public class BranchContext : IBranchContext
{
    private readonly NavigationManager _navigationManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    private readonly ISnackbar _snackbar;
    private readonly IJSRuntime _jsRuntime;

    private int? _cachedBranchId;

    public BranchContext(
        NavigationManager navigationManager,
        IHttpContextAccessor httpContextAccessor,
        AuthenticationStateProvider authenticationStateProvider,
        UserManager<ApplicationUser> userManager,
        IDbContextFactory<ApplicationDbContext> dbFactory,
        ISnackbar snackbar,
        IJSRuntime jsRuntime)
    {
        _navigationManager = navigationManager;
        _httpContextAccessor = httpContextAccessor;
        _authenticationStateProvider = authenticationStateProvider;
        _userManager = userManager;
        _dbFactory = dbFactory;
        _snackbar = snackbar;
        _jsRuntime = jsRuntime;
    }

    public async Task<int> GetBranchIdAsync()
    {
        if (_cachedBranchId.HasValue)
        {
            return _cachedBranchId.Value;
        }

        _cachedBranchId = await ResolveBranchIdAsync();
        return _cachedBranchId.Value;
    }

    public async Task SetBranchIdAsync(int branchId)
    {
        // Simple cookie setting for now. In production, use DataProtection or secure cookie options.
        try
        {
            await _jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'mf.branch={branchId}; path=/; max-age=31536000; SameSite=Strict'");
        }
        catch
        {
            // Ignore if JS not available
        }
    }

    private async Task<int> ResolveBranchIdAsync()
    {
        using var context = await _dbFactory.CreateDbContextAsync();

        // 1. Query String
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var branchIdVal) && int.TryParse(branchIdVal, out var branchId))
        {
            if (await IsMemberAsync(context, branchId)) return branchId;

            // Unauthorized
            try {
                _snackbar.Add("Unauthorized branch access requested. Falling back to default.", Severity.Warning);
            } catch { /* specific context might not allow snackbar */ }
        }

        // 2. Cookie
        if (_httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue("mf.branch", out var cookieBranchIdVal) == true && int.TryParse(cookieBranchIdVal, out var cookieBranchId))
        {
             if (await IsMemberAsync(context, cookieBranchId)) return cookieBranchId;
        }

        // 3. User Default
        var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(user);

            var appUser = await context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

            if (appUser?.DefaultBranchId != null)
            {
                 if (await IsMemberAsync(context, appUser.DefaultBranchId.Value))
                    return appUser.DefaultBranchId.Value;
            }

            // 4. First available membership
            var firstMembership = await context.BranchMemberships
                .Where(m => m.UserId == userId)
                .Select(m => m.BranchId)
                .FirstOrDefaultAsync();

            if (firstMembership != 0) return firstMembership;
        }

        return 0;
    }

    private async Task<bool> IsMemberAsync(ApplicationDbContext context, int branchId)
    {
         var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
         var user = authState.User;
         if (user.Identity?.IsAuthenticated != true) return false;

         var userId = _userManager.GetUserId(user);
         if (userId == null) return false;

         return await context.BranchMemberships.AsNoTracking().AnyAsync(m => m.UserId == userId && m.BranchId == branchId);
    }
}
