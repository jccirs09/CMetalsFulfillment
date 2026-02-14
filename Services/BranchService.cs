using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public class BranchService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        public async Task<List<Branch>> GetActiveBranchesAsync()
        {
            using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.Branches
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<Branch?> GetBranchByIdAsync(int id)
        {
             using var context = await dbContextFactory.CreateDbContextAsync();
             return await context.Branches.FindAsync(id);
        }

        public async Task<Branch?> GetBranchByCodeAsync(string code)
        {
            using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.Branches.FirstOrDefaultAsync(b => b.Code == code);
        }

        public async Task<List<Branch>> GetUserBranchesAsync(string userId)
        {
            using var context = await dbContextFactory.CreateDbContextAsync();
            return await context.UserBranchMemberships
                .Where(m => m.UserId == userId && m.IsActive && m.Branch.IsActive)
                .Select(m => m.Branch)
                .OrderBy(b => b.Name)
                .ToListAsync();
        }

        public async Task<int?> GetUserDefaultBranchIdAsync(string userId)
        {
             using var context = await dbContextFactory.CreateDbContextAsync();
             var membership = await context.UserBranchMemberships
                 .FirstOrDefaultAsync(m => m.UserId == userId && m.IsDefaultForUser && m.IsActive);
             return membership?.BranchId;
        }
    }
}
