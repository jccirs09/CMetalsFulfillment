using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services.Configuration;

public class ShippingConfigService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IShippingConfigService
{
    public async Task<List<Truck>> GetTrucksAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Trucks.AsNoTracking().Where(t => t.BranchId == branchId).ToListAsync();
    }

    public async Task CreateTruckAsync(Truck truck)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        context.Trucks.Add(truck);
        await context.SaveChangesAsync();
    }

    public async Task UpdateTruckAsync(Truck truck)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var existing = await context.Trucks.FirstOrDefaultAsync(t => t.Id == truck.Id && t.BranchId == truck.BranchId);
        if (existing == null) throw new InvalidOperationException("Truck not found");

        context.Entry(existing).OriginalValues["Version"] = truck.Version;

        existing.Name = truck.Name;
        existing.LicensePlate = truck.LicensePlate;
        existing.MaxWeightLbs = truck.MaxWeightLbs;
        existing.Status = truck.Status;

        await context.SaveChangesAsync();
    }

    public async Task DeleteTruckAsync(int id, int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var truck = await context.Trucks.FirstOrDefaultAsync(t => t.Id == id && t.BranchId == branchId);
        if (truck != null)
        {
            context.Trucks.Remove(truck);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<ShippingRegion>> GetRegionsAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.ShippingRegions.AsNoTracking().Where(r => r.BranchId == branchId).ToListAsync();
    }

    public async Task<List<ShippingGroup>> GetGroupsAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.ShippingGroups.AsNoTracking().Where(g => g.BranchId == branchId).ToListAsync();
    }

    public async Task<List<ShippingFsaRule>> GetFsaRulesAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.ShippingFsaRules
            .AsNoTracking()
            .Include(r => r.ShippingRegion)
            .Include(g => g.ShippingGroup)
            .Where(r => r.BranchId == branchId)
            .ToListAsync();
    }

    public async Task CreateRegionAsync(ShippingRegion region)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        context.ShippingRegions.Add(region);
        await context.SaveChangesAsync();
    }

    public async Task CreateGroupAsync(ShippingGroup group)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        context.ShippingGroups.Add(group);
        await context.SaveChangesAsync();
    }

    public async Task CreateFsaRuleAsync(ShippingFsaRule rule)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        context.ShippingFsaRules.Add(rule);
        await context.SaveChangesAsync();
    }

    // Logic: FSA auto-assigns Region + Group
    public async Task<(ShippingRegion? Region, ShippingGroup? Group)> GetShippingDetailsAsync(string fsa, int branchId)
    {
        if (string.IsNullOrWhiteSpace(fsa) || fsa.Length < 3) return (null, null);

        using var context = await dbContextFactory.CreateDbContextAsync();

        var normalizedFsa = fsa.ToUpperInvariant().Replace(" ", "");
        var prefix = normalizedFsa.Substring(0, 3);

        // Fetch rules matching the prefix or sub-prefix
        // Since FSA rules are typically prefix based (e.g. V4G), match starts with.
        var rules = await context.ShippingFsaRules
            .AsNoTracking()
            .Include(r => r.ShippingRegion)
            .Include(r => r.ShippingGroup)
            .Where(r => r.BranchId == branchId)
            .ToListAsync();

        // In-memory match for longest prefix
        var match = rules
            .Where(r => normalizedFsa.StartsWith(r.FsaPrefix.ToUpperInvariant()))
            .OrderByDescending(r => r.FsaPrefix.Length)
            .FirstOrDefault();

        return (match?.ShippingRegion, match?.ShippingGroup);
    }

    // Logic: ShippingMethod from FOB
    public string GetShippingMethod(string fob)
    {
        if (string.IsNullOrWhiteSpace(fob)) return "Delivery"; // Default

        // "ShippingMethod derived strictly from FOB"
        // Rule: FOB Origin usually implies buyer pays/arranges -> Customer Pickup (or Collect)
        // Rule: FOB Destination implies seller pays/arranges -> Delivery (Prepaid)
        // Simplistic rule for this phase:
        if (fob.Contains("Origin", StringComparison.OrdinalIgnoreCase))
        {
            return "CustomerPickup";
        }

        return "Delivery";
    }
}
