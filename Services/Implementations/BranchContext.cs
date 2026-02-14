using CMetalsFulfillment.Data;
using CMetalsFulfillment.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMetalsFulfillment.Services.Implementations;

public class BranchContext : IBranchContext
{
    private readonly NavigationManager _navigationManager;
    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;

    private int? _currentBranchId;

    public BranchContext(
        NavigationManager navigationManager,
        AuthenticationStateProvider authStateProvider,
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager)
    {
        _navigationManager = navigationManager;
        _authStateProvider = authStateProvider;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<int> GetBranchIdAsync()
    {
        if (_currentBranchId.HasValue && _currentBranchId.Value > 0)
        {
            return _currentBranchId.Value;
        }

        // 1. Query String
        var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        if (query.TryGetValue("branchId", out var branchIdStr) && int.TryParse(branchIdStr, out var branchId))
        {
            _currentBranchId = branchId;
            return branchId;
        }

        // 2. User Default (Persisted Selection) or Membership Default
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            // We need to fetch the user ID safely
            var userId = _userManager.GetUserId(user);
            if (userId != null)
            {
                // We fetch the user with memberships
                // Note: We use AsNoTracking if we don't intend to modify, but here we might set DefaultBranchId later.
                // However, for GetBranchIdAsync, reading is fine.
                var appUser = await _dbContext.Users
                    .Include(u => u.BranchMemberships)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (appUser != null)
                {
                    // 2. Persisted Selection (DefaultBranchId)
                    if (appUser.DefaultBranchId.HasValue)
                    {
                        _currentBranchId = appUser.DefaultBranchId.Value;
                        return _currentBranchId.Value;
                    }

                    // 3. Membership Default
                    var defaultMembership = appUser.BranchMemberships.FirstOrDefault(m => m.DefaultForUser);
                    if (defaultMembership != null)
                    {
                        _currentBranchId = defaultMembership.BranchId;
                        return _currentBranchId.Value;
                    }

                    // Fallback: Any membership
                    var firstMembership = appUser.BranchMemberships.FirstOrDefault();
                    if (firstMembership != null)
                    {
                        _currentBranchId = firstMembership.BranchId;
                        return _currentBranchId.Value;
                    }
                }
            }
        }

        return 0; // No branch context
    }

    public async Task SetBranchIdAsync(int branchId)
    {
        _currentBranchId = branchId;

        // Persist to DB
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var userId = _userManager.GetUserId(user);
            if (userId != null)
            {
                var appUser = await _dbContext.Users.FindAsync(userId);
                if (appUser != null)
                {
                    appUser.DefaultBranchId = branchId;
                    await _dbContext.SaveChangesAsync();
                }
            }
        }
    }
}
