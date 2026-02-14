using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMetalsFulfillment.Features.Auth
{
    public class BranchContext : IBranchContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IServiceProvider _serviceProvider;

        private int? _cachedBranchId;

        public BranchContext(
            IHttpContextAccessor httpContextAccessor,
            NavigationManager navigationManager,
            AuthenticationStateProvider authenticationStateProvider,
            IServiceProvider serviceProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
            _serviceProvider = serviceProvider;
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

        private async Task<int> ResolveBranchIdAsync()
        {
            // 1. Try Query/Header/Route from HttpContext
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                if (httpContext.Request.Query.TryGetValue("branchId", out var queryVal) && int.TryParse(queryVal, out int bid)) return bid;
                if (httpContext.Request.Headers.TryGetValue("X-BranchId", out var headerVal) && int.TryParse(headerVal, out bid)) return bid;
                if (httpContext.Request.RouteValues.TryGetValue("branchId", out var routeObj) && routeObj is string routeStr && int.TryParse(routeStr, out bid)) return bid;
            }

            // 2. Try NavigationManager (Blazor)
            try
            {
                var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
                if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var navVal) && int.TryParse(navVal, out int navBid))
                    return navBid;
            }
            catch { /* Ignore */ }

            // 3. Fallback to User Default
            return await GetUserDefaultBranchIdAsync();
        }

        private async Task<int> GetUserDefaultBranchIdAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    // Try to get from User entity directly first
                    var userEntity = await db.Users.FindAsync(userId);
                    if (userEntity?.DefaultBranchId != null)
                    {
                        return userEntity.DefaultBranchId.Value;
                    }

                    // Fallback to memberships
                    var membership = await db.UserBranchMemberships
                        .Where(m => m.UserId == userId && m.IsActive && m.DefaultForUser)
                        .FirstOrDefaultAsync();

                    if (membership != null) return membership.BranchId;

                    var any = await db.UserBranchMemberships
                        .Where(m => m.UserId == userId && m.IsActive)
                        .FirstOrDefaultAsync();

                    if (any != null) return any.BranchId;
                }
            }
            return 0;
        }
    }
}
