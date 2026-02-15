using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMetalsFulfillment.Services
{
    public interface IBranchContext
    {
        Task<int> GetBranchIdAsync();
        Task<Branch?> GetBranchAsync();
        Task SetBranchAsync(int branchId);
    }

    public class BranchContext : IBranchContext
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly AuthenticationStateProvider _authProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NavigationManager _nav;
        private int? _cachedBranchId;
        private Branch? _cachedBranch;

        public BranchContext(IDbContextFactory<ApplicationDbContext> contextFactory,
                             AuthenticationStateProvider authProvider,
                             IHttpContextAccessor httpContextAccessor,
                             NavigationManager nav)
        {
            _contextFactory = contextFactory;
            _authProvider = authProvider;
            _httpContextAccessor = httpContextAccessor;
            _nav = nav;
        }

        public async Task<int> GetBranchIdAsync()
        {
            if (_cachedBranchId.HasValue)
                return _cachedBranchId.Value;

            await ResolveBranchAsync();
            return _cachedBranchId ?? 0;
        }

        public async Task<Branch?> GetBranchAsync()
        {
            if (_cachedBranch != null) return _cachedBranch;
            await ResolveBranchAsync();
            return _cachedBranch;
        }

        private async Task ResolveBranchAsync()
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user?.Identity?.IsAuthenticated != true)
                return;

            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return;

            using var context = await _contextFactory.CreateDbContextAsync();

            // 1. Query String ?branchId=
            var uri = _nav.ToAbsoluteUri(_nav.Uri);
            if (Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query).TryGetValue("branchId", out var branchIdVal)
                && int.TryParse(branchIdVal, out var qBranchId))
            {
                if (await CheckMembershipAsync(context, userId, qBranchId))
                {
                    await SetBranchCache(context, qBranchId);
                    return;
                }
            }

            // 2. Cookie mf.branch
            if (_httpContextAccessor.HttpContext?.Request.Cookies.TryGetValue("mf.branch", out var cookieBranchVal) == true
                && int.TryParse(cookieBranchVal, out var cBranchId))
            {
                if (await CheckMembershipAsync(context, userId, cBranchId))
                {
                    await SetBranchCache(context, cBranchId);
                    return;
                }
            }

            // 3. Default Membership
            var defaultMembership = await context.UserBranchMemberships
                .Where(m => m.UserId == userId && m.IsDefaultForUser && m.IsActive)
                .Select(m => m.BranchId)
                .FirstOrDefaultAsync();

            if (defaultMembership > 0)
            {
                await SetBranchCache(context, defaultMembership);
                return;
            }

            // 4. Any Membership
            var anyMembership = await context.UserBranchMemberships
                .Where(m => m.UserId == userId && m.IsActive)
                .Select(m => m.BranchId)
                .FirstOrDefaultAsync();

             if (anyMembership > 0)
            {
                await SetBranchCache(context, anyMembership);
                return;
            }

            // No access
        }

        private async Task<bool> CheckMembershipAsync(ApplicationDbContext context, string userId, int branchId)
        {
            return await context.UserBranchMemberships
                .AnyAsync(m => m.UserId == userId && m.BranchId == branchId && m.IsActive);
        }

        private async Task SetBranchCache(ApplicationDbContext context, int branchId)
        {
            _cachedBranchId = branchId;
            _cachedBranch = await context.Branches.FindAsync(branchId);
        }

        public async Task SetBranchAsync(int branchId)
        {
             // This is usually handled via UI (cookie + reload), but service can update cache if needed mid-request
             // However, for strict consistency, reload is preferred.
             // We'll update cache locally.
             using var context = await _contextFactory.CreateDbContextAsync();
             await SetBranchCache(context, branchId);
        }
    }
}
