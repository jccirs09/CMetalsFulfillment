using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace CMetalsFulfillment.Services;

public class BranchContext(
    NavigationManager navigationManager,
    IJSRuntime jsRuntime,
    AuthenticationStateProvider authStateProvider,
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IDataProtectionProvider dataProtectionProvider) : IBranchContext
{
    private int? _branchId;
    private readonly IDataProtector _protector = dataProtectionProvider.CreateProtector("BranchContext.Cookie");

    public async Task<int> GetBranchIdAsync()
    {
        // 1. Query String (Priority)
        var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
        if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var branchIdVal) && int.TryParse(branchIdVal, out var bid))
        {
             // Validate against DB
             if (await IsValidBranchAsync(bid))
             {
                if (_branchId != bid)
                {
                    _branchId = bid;
                    await SetBranchCookieAsync(bid);
                }
                return bid;
             }
             // If invalid, do NOT return it. Fall through to other methods (Fallback).
        }

        // 2. In-Memory Cache (Circuit Scope)
        if (_branchId.HasValue)
        {
            return _branchId.Value;
        }

        // 3. Cookie
        try
        {
            var cookie = await jsRuntime.InvokeAsync<string>("eval", "document.cookie.split('; ').find(row => row.startsWith('mf.branch='))?.split('=')[1]");
            if (!string.IsNullOrEmpty(cookie))
            {
                // Decrypt
                try
                {
                    var decrypted = _protector.Unprotect(cookie);
                    if (int.TryParse(decrypted, out var cookieBid) && await IsValidBranchAsync(cookieBid))
                    {
                        _branchId = cookieBid;
                        return cookieBid;
                    }
                }
                catch
                {
                    // Invalid cookie or decryption failed
                }
            }
        }
        catch { /* Prerendering or disconnected */ }

        // 4. Default Claim
        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var claim = user.FindFirst("mf:defaultBranchId");
            if (claim != null && int.TryParse(claim.Value, out var defaultBid) && await IsValidBranchAsync(defaultBid))
            {
                _branchId = defaultBid;
                return defaultBid;
            }
        }

        return 0;
    }

    private async Task<bool> IsValidBranchAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Branches.AnyAsync(b => b.Id == branchId);
    }

    private async Task SetBranchCookieAsync(int branchId)
    {
        try
        {
            var encrypted = _protector.Protect(branchId.ToString());
            // Set cookie for 7 days
            await jsRuntime.InvokeVoidAsync("eval", $"document.cookie = 'mf.branch={encrypted}; path=/; max-age=604800; samesite=strict'");
        }
        catch { /* Ignore */ }
    }
}
