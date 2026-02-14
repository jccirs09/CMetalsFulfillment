using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class StationService : IStationService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public StationService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<PickPackStation>> GetStationsAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.PickPackStations.Where(s => s.BranchId == branchId).ToListAsync();
    }

    public async Task<PickPackStation?> GetStationAsync(int id, int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.PickPackStations.FirstOrDefaultAsync(s => s.Id == id && s.BranchId == branchId);
    }

    public async Task<PickPackStation> CreateStationAsync(PickPackStation station)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        if (await db.PickPackStations.AnyAsync(s => s.BranchId == station.BranchId && s.StationCode == station.StationCode))
        {
            throw new InvalidOperationException("Station Code already exists for this branch.");
        }

        db.PickPackStations.Add(station);
        await db.SaveChangesAsync();
        return station;
    }

    public async Task<PickPackStation> UpdateStationAsync(PickPackStation station)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.PickPackStations.FirstOrDefaultAsync(s => s.Id == station.Id && s.BranchId == station.BranchId);
        if (existing == null) throw new KeyNotFoundException("Station not found.");

        existing.StationName = station.StationName;
        existing.IsActive = station.IsActive;
        existing.Notes = station.Notes;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return existing;
    }
}
