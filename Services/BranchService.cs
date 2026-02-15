using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public class BranchService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public BranchService(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<Branch>> GetAllBranchesAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.Branches.OrderBy(b => b.Code).ToListAsync();
        }

        public async Task<List<Branch>> GetMyBranchesAsync(string userId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            return await context.UserBranchMemberships
                .Where(m => m.UserId == userId && m.IsActive)
                .Include(m => m.Branch)
                .Select(m => m.Branch)
                .OrderBy(b => b.Code)
                .ToListAsync();
        }

        public async Task SetDefaultBranchAsync(string userId, int branchId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var memberships = await context.UserBranchMemberships
                .Where(m => m.UserId == userId)
                .ToListAsync();

            foreach (var m in memberships)
            {
                m.IsDefaultForUser = (m.BranchId == branchId);
            }

            await context.SaveChangesAsync();
        }
    }
}
