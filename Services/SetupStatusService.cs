using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public class SetupStatusService(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        public async Task<bool> IsSetupCompleteAsync(int branchId)
        {
            using var context = await dbContextFactory.CreateDbContextAsync();

            // Check Branch exists + active
            var branch = await context.Branches.FindAsync(branchId);
            if (branch == null || !branch.IsActive) return false;

            // Check at least one BranchAdmin assigned
            var hasAdmin = await context.UserBranchRoles
                .AnyAsync(r => r.BranchId == branchId && r.RoleName == "BranchAdmin");

            if (!hasAdmin) return false;

            // Phase 2 placeholders - always fail for now as they are not implemented
            // Returns false to indicate setup is incomplete (as per requirements for missing configs)
            return false;
        }

        public async Task<List<string>> GetMissingSetupStepsAsync(int branchId)
        {
             using var context = await dbContextFactory.CreateDbContextAsync();
             var missing = new List<string>();

             var branch = await context.Branches.FindAsync(branchId);
             if (branch == null || !branch.IsActive) missing.Add("Branch not active or missing");

             var hasAdmin = await context.UserBranchRoles.AnyAsync(r => r.BranchId == branchId && r.RoleName == "BranchAdmin");
             if (!hasAdmin) missing.Add("No Branch Admin assigned");

             // Phase 2 placeholders
             missing.Add("Machines not configured");
             missing.Add("PickPackStations not configured");
             missing.Add("ShiftTemplates not configured");
             missing.Add("Trucks not configured");
             missing.Add("ShippingFsaRules not configured");

             return missing;
        }
    }
}
