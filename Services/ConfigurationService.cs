using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class ConfigurationService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public ConfigurationService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Machine>> GetMachinesAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Machines.Where(x => x.BranchId == branchId).ToListAsync();
    }

    public async Task SaveMachineAsync(Machine machine)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        if (machine.Id == 0)
        {
            db.Machines.Add(machine);
        }
        else
        {
            db.Machines.Update(machine);
        }
        await db.SaveChangesAsync();
    }

    public async Task<List<PickPackStation>> GetStationsAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.PickPackStations.Where(x => x.BranchId == branchId).ToListAsync();
    }

    public async Task SaveStationAsync(PickPackStation station)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        if (station.Id == 0)
        {
            db.PickPackStations.Add(station);
        }
        else
        {
            db.PickPackStations.Update(station);
        }
        await db.SaveChangesAsync();
    }

    public async Task<List<ShiftTemplate>> GetShiftsAsync(int branchId)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         return await db.ShiftTemplates.Where(x => x.BranchId == branchId).ToListAsync();
    }

    public async Task SaveShiftAsync(ShiftTemplate shift)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         if (shift.Id == 0)
         {
             db.ShiftTemplates.Add(shift);
         }
         else
         {
             db.ShiftTemplates.Update(shift);
         }
         await db.SaveChangesAsync();
    }

    public async Task<List<Truck>> GetTrucksAsync(int branchId)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         return await db.Trucks.Where(x => x.BranchId == branchId).ToListAsync();
    }

    public async Task SaveTruckAsync(Truck truck)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         if (truck.Id == 0)
         {
             db.Trucks.Add(truck);
         }
         else
         {
             db.Trucks.Update(truck);
         }
         await db.SaveChangesAsync();
    }

    public async Task<List<ShippingRegion>> GetRegionsAsync(int branchId)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         return await db.ShippingRegions.Where(x => x.BranchId == branchId).ToListAsync();
    }

    public async Task SaveRegionAsync(ShippingRegion region)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         if (region.Id == 0)
         {
             db.ShippingRegions.Add(region);
         }
         else
         {
             db.ShippingRegions.Update(region);
         }
         await db.SaveChangesAsync();
    }

    public async Task<List<ShippingGroup>> GetGroupsAsync(int branchId)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         return await db.ShippingGroups.Where(x => x.BranchId == branchId).ToListAsync();
    }

    public async Task SaveGroupAsync(ShippingGroup group)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         if (group.Id == 0)
         {
             db.ShippingGroups.Add(group);
         }
         else
         {
             db.ShippingGroups.Update(group);
         }
         await db.SaveChangesAsync();
    }

    public async Task<List<NonWorkingDay>> GetNonWorkingDaysAsync(int branchId)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         return await db.NonWorkingDays.Where(x => x.BranchId == branchId).OrderBy(x => x.Date).ToListAsync();
    }

    public async Task SaveNonWorkingDayAsync(NonWorkingDay day)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         if (day.Id == 0)
         {
             db.NonWorkingDays.Add(day);
         }
         else
         {
             db.NonWorkingDays.Update(day);
         }
         await db.SaveChangesAsync();
    }

    public async Task DeleteNonWorkingDayAsync(NonWorkingDay day)
    {
         using var db = await _dbFactory.CreateDbContextAsync();
         db.NonWorkingDays.Remove(day);
         await db.SaveChangesAsync();
    }
}
