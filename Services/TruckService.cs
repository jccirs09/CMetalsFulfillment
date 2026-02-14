using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class TruckService : ITruckService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public TruckService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Truck>> GetTrucksAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Trucks.Where(t => t.BranchId == branchId).ToListAsync();
    }

    public async Task<Truck?> GetTruckAsync(int id, int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Trucks.FirstOrDefaultAsync(t => t.Id == id && t.BranchId == branchId);
    }

    public async Task<Truck> CreateTruckAsync(Truck truck)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        if (await db.Trucks.AnyAsync(t => t.BranchId == truck.BranchId && t.TruckCode == truck.TruckCode))
        {
            throw new InvalidOperationException("Truck Code already exists for this branch.");
        }

        db.Trucks.Add(truck);
        await db.SaveChangesAsync();
        return truck;
    }

    public async Task<Truck> UpdateTruckAsync(Truck truck)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.Trucks.FirstOrDefaultAsync(t => t.Id == truck.Id && t.BranchId == truck.BranchId);
        if (existing == null) throw new KeyNotFoundException("Truck not found.");

        existing.Description = truck.Description;
        existing.MaxWeightLbs = truck.MaxWeightLbs;
        existing.IsActive = truck.IsActive;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return existing;
    }
}
