using CMetalsFulfillment.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public class UserAdminService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAdminService(IDbContextFactory<ApplicationDbContext> contextFactory, UserManager<ApplicationUser> userManager)
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Users.OrderBy(u => u.UserName).ToListAsync();
        }

        public async Task<ApplicationUser?> GetUserAsync(string userId)
        {
             using var context = await _contextFactory.CreateDbContextAsync();
             return await context.Users.FindAsync(userId);
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<List<UserBranchMembership>> GetMembershipsAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserBranchMemberships
                .Include(m => m.Branch)
                .Where(m => m.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateMembershipAsync(string userId, int branchId, bool isActive, bool isDefault)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var membership = await context.UserBranchMemberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.BranchId == branchId);

            if (membership == null)
            {
                membership = new UserBranchMembership
                {
                    UserId = userId,
                    BranchId = branchId,
                    AddedAtUtc = DateTime.UtcNow
                };
                context.UserBranchMemberships.Add(membership);
            }

            membership.IsActive = isActive;
            membership.IsDefaultForUser = isDefault;

            if (isDefault)
            {
                // Unset other defaults
                var others = await context.UserBranchMemberships
                    .Where(m => m.UserId == userId && m.BranchId != branchId && m.IsDefaultForUser)
                    .ToListAsync();
                foreach (var o in others) o.IsDefaultForUser = false;
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<UserBranchRole>> GetBranchRolesAsync(string userId, int branchId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserBranchRoles
                .Where(r => r.UserId == userId && r.BranchId == branchId)
                .ToListAsync();
        }

        public async Task AddBranchRoleAsync(string userId, int branchId, string roleName, string assignedByUserId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            if (!await context.UserBranchRoles.AnyAsync(r => r.UserId == userId && r.BranchId == branchId && r.RoleName == roleName))
            {
                context.UserBranchRoles.Add(new UserBranchRole
                {
                    UserId = userId,
                    BranchId = branchId,
                    RoleName = roleName,
                    AssignedByUserId = assignedByUserId,
                    AssignedAtUtc = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
        }

        public async Task RemoveBranchRoleAsync(int roleId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var role = await context.UserBranchRoles.FindAsync(roleId);
            if (role != null)
            {
                context.UserBranchRoles.Remove(role);
                await context.SaveChangesAsync();
            }
        }
    }
}
