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
        gates["Machine"] = false;

        // 3. PickPackStation
        gates["PickPackStation"] = false;

        // 4. ShiftTemplate
        gates["ShiftTemplate"] = false;

        // 5. Truck
        gates["Truck"] = false;

        // 6. ShippingFsaRule
        gates["ShippingFsaRule"] = false;

        // 7. ShippingFobMapping
        gates["ShippingFobMapping"] = false;

        return new SetupStatus { Gates = gates };
    }
}
