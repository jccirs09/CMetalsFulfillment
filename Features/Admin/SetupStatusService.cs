using CMetalsFulfillment.Data;
using CMetalsFulfillment.Features.Auth;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Features.Admin
{
    public class SetupStatusService
    {
        private readonly ApplicationDbContext _db;
        private readonly IBranchContext _branchContext;

        public SetupStatusService(ApplicationDbContext db, IBranchContext branchContext)
        {
            _db = db;
            _branchContext = branchContext;
        }

        public async Task<BranchSetupStatusDto> GetStatusAsync()
        {
            var branchId = _branchContext.BranchId;
            if (branchId == 0) return new BranchSetupStatusDto();

            var hasMachines = await _db.Machines.AnyAsync(m => m.BranchId == branchId && m.IsActive);
            var hasStations = await _db.PickPackStations.AnyAsync(p => p.BranchId == branchId && p.IsActive);
            var hasShifts = await _db.ShiftTemplates.AnyAsync(s => s.BranchId == branchId && s.IsActive);

            var hasRegions = await _db.ShippingRegions.AnyAsync(r => r.BranchId == branchId && r.IsActive);
            var hasFsaRules = await _db.ShippingFsaRules.AnyAsync(f => f.BranchId == branchId && f.IsActive);
            var hasShippingRules = hasRegions && hasFsaRules;

            var hasItems = await _db.Items.AnyAsync(i => i.BranchId == branchId && i.IsActive);

            return new BranchSetupStatusDto
            {
                BranchId = branchId,
                HasMachines = hasMachines,
                HasPickPackStations = hasStations,
                HasShiftTemplates = hasShifts,
                HasShippingRules = hasShippingRules,
                HasItemMaster = hasItems
            };
        }
    }

    public class BranchSetupStatusDto
    {
        public int BranchId { get; set; }
        public bool HasMachines { get; set; }
        public bool HasPickPackStations { get; set; }
        public bool HasShiftTemplates { get; set; }
        public bool HasShippingRules { get; set; }
        public bool HasItemMaster { get; set; }

        public bool IsComplete => HasMachines && HasPickPackStations && HasShiftTemplates && HasShippingRules && HasItemMaster;
    }
}
