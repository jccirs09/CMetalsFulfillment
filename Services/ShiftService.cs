using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class ShiftService : IShiftService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public ShiftService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<ShiftTemplate>> GetShiftsAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ShiftTemplates.Where(s => s.BranchId == branchId).ToListAsync();
    }

    public async Task<ShiftTemplate?> GetShiftAsync(int id, int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ShiftTemplates.FirstOrDefaultAsync(s => s.Id == id && s.BranchId == branchId);
    }

    public async Task<ShiftTemplate> CreateShiftAsync(ShiftTemplate shift)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        if (await db.ShiftTemplates.AnyAsync(s => s.BranchId == shift.BranchId && s.ShiftCode == shift.ShiftCode))
        {
            throw new InvalidOperationException("Shift Code already exists for this branch.");
        }

        db.ShiftTemplates.Add(shift);
        await db.SaveChangesAsync();
        return shift;
    }

    public async Task<ShiftTemplate> UpdateShiftAsync(ShiftTemplate shift)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.ShiftTemplates.FirstOrDefaultAsync(s => s.Id == shift.Id && s.BranchId == shift.BranchId);
        if (existing == null) throw new KeyNotFoundException("Shift not found.");

        existing.Name = shift.Name;
        existing.StartLocalTime = shift.StartLocalTime;
        existing.EndLocalTime = shift.EndLocalTime;
        existing.BreakMinutes = shift.BreakMinutes;
        existing.IsActive = shift.IsActive;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return existing;
    }
}
