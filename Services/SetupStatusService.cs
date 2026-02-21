using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public interface ISetupStatusService
    {
        Task<bool> IsSetupCompleteAsync(int branchId);
        Task<Dictionary<string, bool>> GetGateStatusAsync(int branchId);
    }

    public class SetupStatusService(IDbContextFactory<ApplicationDbContext> dbFactory) : ISetupStatusService
    {
        public async Task<bool> IsSetupCompleteAsync(int branchId)
        {
            var gates = await GetGateStatusAsync(branchId);
            return gates.Values.All(v => v);
        }

        public async Task<Dictionary<string, bool>> GetGateStatusAsync(int branchId)
        {
            using var context = await dbFactory.CreateDbContextAsync();
            var status = new Dictionary<string, bool>();

            // 1. Branch Active
            var branch = await context.Branches.FirstOrDefaultAsync(b => b.Id == branchId);
            status["BranchActive"] = branch?.IsActive ?? false;

            // 2. Branch Admin Exists
            var adminExists = await context.UserBranchRoles
                .AnyAsync(r => r.BranchId == branchId && r.RoleName == "BranchAdmin");
            status["BranchAdminExists"] = adminExists;

            // Phase 2 Gates (Hardcoded false for now as tables don't exist)
            status["MachinesConfigured"] = false;
            status["PickPackStationsConfigured"] = false;
            status["ShiftTemplatesConfigured"] = false;
            status["TrucksConfigured"] = false;
            status["ShippingFsaRulesConfigured"] = false;

            return status;
        }
    }
}
