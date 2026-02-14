using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public interface IUserBranchService
    {
        Task<List<UserBranchMembership>> GetUserMembershipsAsync(string userId);
        Task<bool> HasMembershipAsync(string userId, int branchId);
        Task SetDefaultBranchAsync(string userId, int branchId);
        Task AddMembershipAsync(string userId, int branchId, bool isDefault);
        Task AssignRoleAsync(string userId, int branchId, string roleName, string assignedByUserId);
        Task<UserBranchMembership?> GetDefaultMembershipAsync(string userId);
    }

    public class UserBranchService(IDbContextFactory<ApplicationDbContext> dbFactory) : IUserBranchService
    {
        public async Task<List<UserBranchMembership>> GetUserMembershipsAsync(string userId)
        {
            using var context = await dbFactory.CreateDbContextAsync();
            return await context.UserBranchMemberships
                .Include(m => m.Branch)
                .Where(m => m.UserId == userId && m.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasMembershipAsync(string userId, int branchId)
        {
            using var context = await dbFactory.CreateDbContextAsync();
            return await context.UserBranchMemberships
                .AnyAsync(m => m.UserId == userId && m.BranchId == branchId && m.IsActive);
        }

        public async Task SetDefaultBranchAsync(string userId, int branchId)
        {
            using var context = await dbFactory.CreateDbContextAsync();
            var memberships = await context.UserBranchMemberships
                .Where(m => m.UserId == userId)
                .ToListAsync();

            foreach (var membership in memberships)
            {
                membership.IsDefaultForUser = (membership.BranchId == branchId);
            }

            await context.SaveChangesAsync();
        }

        public async Task<UserBranchMembership?> GetDefaultMembershipAsync(string userId)
        {
            using var context = await dbFactory.CreateDbContextAsync();
            return await context.UserBranchMemberships
                .Include(m => m.Branch)
                .FirstOrDefaultAsync(m => m.UserId == userId && m.IsDefaultForUser && m.IsActive);
        }

        public async Task AddMembershipAsync(string userId, int branchId, bool isDefault)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            // Check if exists
            var existing = await context.UserBranchMemberships
                .FirstOrDefaultAsync(m => m.UserId == userId && m.BranchId == branchId);

            if (existing != null)
            {
                if (!existing.IsActive)
                {
                    existing.IsActive = true;
                }
                // Handle default flag logic if needed, but usually strictly handled
            }
            else
            {
                var membership = new UserBranchMembership
                {
                    UserId = userId,
                    BranchId = branchId,
                    IsDefaultForUser = isDefault,
                    IsActive = true,
                    AddedAtUtc = DateTime.UtcNow
                };
                context.UserBranchMemberships.Add(membership);
            }

            if (isDefault)
            {
                // Ensure no other default exists or update existing ones
                var otherDefaults = await context.UserBranchMemberships
                    .Where(m => m.UserId == userId && m.IsDefaultForUser && m.BranchId != branchId)
                    .ToListAsync();

                foreach (var other in otherDefaults)
                {
                    other.IsDefaultForUser = false;
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task AssignRoleAsync(string userId, int branchId, string roleName, string assignedByUserId)
        {
            using var context = await dbFactory.CreateDbContextAsync();

            // Check if exists
            var exists = await context.UserBranchRoles
                .AnyAsync(r => r.UserId == userId && r.BranchId == branchId && r.RoleName == roleName);

            if (!exists)
            {
                var role = new UserBranchRole
                {
                    UserId = userId,
                    BranchId = branchId,
                    RoleName = roleName,
                    AssignedByUserId = assignedByUserId,
                    AssignedAtUtc = DateTime.UtcNow
                };
                context.UserBranchRoles.Add(role);
                await context.SaveChangesAsync();
            }
        }
    }
}
