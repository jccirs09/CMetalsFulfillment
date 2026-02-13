using System.Security.Claims;
using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Features.Auth
{
    public class BranchContext : IBranchContext, IDisposable
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly NavigationManager _navigationManager;

        private int? _cachedBranchId;

        public BranchContext(
            IHttpContextAccessor httpContextAccessor,
            AuthenticationStateProvider authStateProvider,
            IServiceScopeFactory scopeFactory,
            NavigationManager navigationManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _authStateProvider = authStateProvider;
            _scopeFactory = scopeFactory;
            _navigationManager = navigationManager;

            _navigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            _cachedBranchId = null;
        }

        public void Dispose()
        {
            _navigationManager.LocationChanged -= OnLocationChanged;
        }

        public int? BranchId
        {
            get
            {
                if (_cachedBranchId.HasValue) return _cachedBranchId.Value;

                _cachedBranchId = ResolveBranchId();
                return _cachedBranchId;
            }
        }

        private int? ResolveBranchId()
        {
            // 1. HttpContext Query/Header
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                if (httpContext.Request.Query.TryGetValue("branchId", out var queryId) && int.TryParse(queryId, out int bid)) return bid;
                if (httpContext.Request.Headers.TryGetValue("X-BranchId", out var headerId) && int.TryParse(headerId, out int hid)) return hid;
            }

            // 2. NavigationManager Query (Blazor)
            try
            {
                var uri = _navigationManager.ToAbsoluteUri(_navigationManager.Uri);
                if (Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var navId) && int.TryParse(navId, out int nid)) return nid;
            }
            catch { }

            // 3. Fallback to User Default
            string? userId = null;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            // Note: retrieving user from AuthStateProvider synchronously is not reliable here if HttpContext is null.
            // We rely on HttpContext for the user identity for now as this is a Scoped service often used in API or initial load.

            if (userId != null)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var membership = db.UserBranchMemberships
                        .AsNoTracking()
                        .FirstOrDefault(m => m.UserId == userId && m.DefaultForUser && m.IsActive);

                    if (membership != null) return membership.BranchId;
                }
                catch
                {
                    // Ignore DB errors during resolution
                }
            }

            return null;
        }
    }
}
