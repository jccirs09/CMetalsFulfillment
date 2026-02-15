using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class SetupStatusService : ISetupStatusService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public SetupStatusService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<SetupStatus> GetStatusAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        var gates = new Dictionary<string, bool>();

        // 1. BranchAdmin check
        var hasBranchAdmin = await db.UserBranchRoles
            .AnyAsync(r => r.BranchId == branchId && r.RoleName == "BranchAdmin");
        gates["BranchAdmin"] = hasBranchAdmin;

        // 2. Machine
        gates["Machine"] = await db.Machines.AnyAsync(m => m.BranchId == branchId && m.IsActive);

        // 3. PickPackStation
        gates["PickPackStation"] = await db.PickPackStations.AnyAsync(s => s.BranchId == branchId && s.IsActive);

        // 4. ShiftTemplate
        gates["ShiftTemplate"] = await db.ShiftTemplates.AnyAsync(s => s.BranchId == branchId && s.IsActive);

        // 5. Truck
        gates["Truck"] = await db.Trucks.AnyAsync(t => t.BranchId == branchId && t.IsActive);

        // 6. ShippingFsaRule
        gates["ShippingFsaRule"] = await db.ShippingFsaRules.AnyAsync(r => r.BranchId == branchId && r.IsActive);

        // 7. ShippingFobMapping
        gates["ShippingFobMapping"] = await db.ShippingFobMappings.AnyAsync(m => m.BranchId == branchId && m.IsActive);

        return new SetupStatus { Gates = gates };
    }
}
