using CMetalsFulfillment.Data;
using CMetalsFulfillment.Features.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CMetalsFulfillment.Features.Admin
{
    public class BranchSetupStatus
    {
        public bool HasMachines { get; set; }
        public bool HasPickPackStations { get; set; }
        public bool HasShiftTemplates { get; set; }
        public bool HasShippingRules { get; set; }
        public bool HasItemMaster { get; set; }

        public bool IsComplete => HasMachines && HasPickPackStations && HasShiftTemplates && HasShippingRules && HasItemMaster;
    }

    public class SetupGateService
    {
        private readonly IBranchContext _branchContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public SetupGateService(IBranchContext branchContext, IServiceScopeFactory scopeFactory)
        {
            _branchContext = branchContext;
            _scopeFactory = scopeFactory;
        }

        public async Task<BranchSetupStatus> GetStatusAsync()
        {
             var status = new BranchSetupStatus();
             if (!_branchContext.BranchId.HasValue) return status;

             var branchId = _branchContext.BranchId.Value;

             using var scope = _scopeFactory.CreateScope();
             var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

             status.HasMachines = await db.Machines.AnyAsync(m => m.BranchId == branchId && m.IsActive);
             status.HasPickPackStations = await db.PickPackStations.AnyAsync(s => s.BranchId == branchId && s.IsActive);
             status.HasShiftTemplates = await db.ShiftTemplates.AnyAsync(s => s.BranchId == branchId && s.IsActive);

             // Check Shipping Rules: At least 1 active region AND 1 active FSA rule
             var hasRegion = await db.ShippingRegions.AnyAsync(r => r.BranchId == branchId && r.IsActive);
             var hasRule = await db.ShippingFsaRules.AnyAsync(r => r.BranchId == branchId && r.IsActive);
             status.HasShippingRules = hasRegion && hasRule;

             // Check Item Master: At least 1 active item
             status.HasItemMaster = await db.Items.AnyAsync(i => i.BranchId == branchId && i.IsActive);

             return status;
        }
    }
}
