using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services.Configuration;

public class CalendarService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : ICalendarService
{
    public async Task<List<NonWorkingDay>> GetNonWorkingDaysAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.NonWorkingDays
            .AsNoTracking()
            .Where(d => d.BranchId == branchId)
            .OrderBy(d => d.Date)
            .ToListAsync();
    }

    public async Task<List<NonWorkingDayOverride>> GetOverridesAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.NonWorkingDayOverrides
            .AsNoTracking()
            .Where(o => o.BranchId == branchId)
            .OrderBy(o => o.Date)
            .ToListAsync();
    }

    public async Task AddNonWorkingDayAsync(NonWorkingDay day)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        context.NonWorkingDays.Add(day);
        await context.SaveChangesAsync();
    }

    public async Task AddOverrideAsync(NonWorkingDayOverride ovr)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        context.NonWorkingDayOverrides.Add(ovr);
        await context.SaveChangesAsync();
    }

    public async Task DeleteNonWorkingDayAsync(int id, int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var day = await context.NonWorkingDays.FirstOrDefaultAsync(d => d.Id == id && d.BranchId == branchId);
        if (day != null)
        {
            context.NonWorkingDays.Remove(day);
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteOverrideAsync(int id, int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var ovr = await context.NonWorkingDayOverrides.FirstOrDefaultAsync(o => o.Id == id && o.BranchId == branchId);
        if (ovr != null)
        {
            context.NonWorkingDayOverrides.Remove(ovr);
            await context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsWorkingDayAsync(DateOnly date, int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();

        // 1. Check Override (Priority)
        // Since sqlite dates are stored as strings (TEXT), comparison should work if stored as ISO-8601 (DateOnly defaults to yyyy-MM-dd)
        var overrideDay = await context.NonWorkingDayOverrides
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.BranchId == branchId && o.Date == date);

        if (overrideDay != null)
        {
            return overrideDay.IsWorkingDay;
        }

        // 2. Check NonWorkingDay
        var nonWorking = await context.NonWorkingDays
            .AsNoTracking()
            .AnyAsync(d => d.BranchId == branchId && d.Date == date);

        if (nonWorking) return false;

        // 3. Check Weekend (Sat/Sun)
        if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
        {
            return false;
        }

        return true;
    }
}
