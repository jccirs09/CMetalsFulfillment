using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public interface IBranchService
    {
        Task<List<Branch>> GetAllActiveBranchesAsync();
        Task<Branch?> GetBranchByIdAsync(int id);
        Task<Branch?> GetBranchByCodeAsync(string code);
    }

    public class BranchService(IDbContextFactory<ApplicationDbContext> dbFactory) : IBranchService
    {
        public async Task<List<Branch>> GetAllActiveBranchesAsync()
        {
            using var context = await dbFactory.CreateDbContextAsync();
            return await context.Branches
                .Where(b => b.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Branch?> GetBranchByIdAsync(int id)
        {
            using var context = await dbFactory.CreateDbContextAsync();
            return await context.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Branch?> GetBranchByCodeAsync(string code)
        {
            using var context = await dbFactory.CreateDbContextAsync();
            return await context.Branches
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Code == code);
        }
    }
}
