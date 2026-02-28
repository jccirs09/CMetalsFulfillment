using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public interface IRoleResolver
    {
        Task<bool> HasRoleAsync(string userId, int branchId, string roleName);
        Task<List<string>> GetRolesAsync(string userId, int branchId);
    }

    public class RoleResolver : IRoleResolver
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        // Simple request-scoped cache
        private readonly Dictionary<(string, int), List<string>> _cache = new();

        public RoleResolver(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> HasRoleAsync(string userId, int branchId, string roleName)
        {
            var roles = await GetRolesAsync(userId, branchId);
            return roles.Contains(roleName) || roles.Contains("SystemAdmin"); // implicitly check SystemAdmin usually
        }

        public async Task<List<string>> GetRolesAsync(string userId, int branchId)
        {
            if (_cache.TryGetValue((userId, branchId), out var cachedRoles))
            {
                return cachedRoles;
            }

            using var context = await _contextFactory.CreateDbContextAsync();
            var roles = await context.UserBranchRoles
                .Where(r => r.UserId == userId && r.BranchId == branchId)
                .Select(r => r.RoleName)
                .ToListAsync();

            // Check if user is SystemAdmin globally (via Identity Role) or special logic?
            // AGENTS.md says "RoleResolver: loads roles for (userId, branchId)".
            // It also mentions "SystemAdmin" role. Usually this is global but stored per branch?
            // "Seed Identity roles: SystemAdmin..Viewer." -> Identity Roles are global.
            // "UserBranchRole (branch roles stored ONLY here)"
            // Wait, standard Identity Roles are in AspNetRoles. UserBranchRole is separate.
            // But checking Identity Role "SystemAdmin" usually implies global access.
            // Let's check AGENTS.md Authorization section:
            // "All role checks use: IRoleResolver.HasRole(userId, branchId, roleName)"
            // "Seed Identity roles: SystemAdmin..Viewer" implies we might use Identity for global roles?
            // But "UserBranchRole (branch roles stored ONLY here)" suggests branch specific.
            // Except SystemAdmin.
            // "SystemAdmin seed user... Add Identity role SystemAdmin."
            // So SystemAdmin has Identity Role "SystemAdmin".
            // Others have branch roles.
            // So we should check Identity Roles for SystemAdmin too.

            // However, RoleResolver is scoped, we can inject UserManager or check Claims.
            // But we don't have UserManager easily in scoped service without context issues in Blazor.
            // We can check the User claims if we have access to AuthenticationState, but RoleResolver doesn't take it here.
            // Let's assume the caller handles the "Global SystemAdmin" check or we query AspNetUserRoles.

            var globalRoles = await context.UserRoles
                .Join(context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                .Where(x => x.UserId == userId)
                .Select(x => x.Name)
                .ToListAsync();

            if (globalRoles.Contains("SystemAdmin"))
            {
                roles.Add("SystemAdmin");
            }

            // De-duplicate
            roles = roles.Distinct().ToList();

            _cache[(userId, branchId)] = roles;
            return roles;
        }
    }
}
