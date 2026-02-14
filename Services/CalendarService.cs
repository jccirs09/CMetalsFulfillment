using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class CalendarService : ICalendarService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    private readonly IAuditService _auditService;

    public CalendarService(IDbContextFactory<ApplicationDbContext> dbFactory, IAuditService auditService)
    {
        _dbFactory = dbFactory;
        _auditService = auditService;
    }

    public async Task<List<NonWorkingDay>> GetNonWorkingDaysAsync(int branchId, DateOnly start, DateOnly end)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.NonWorkingDays
            .Where(n => n.BranchId == branchId && n.DateLocal >= start && n.DateLocal <= end && n.IsActive)
            .ToListAsync();
    }

    public async Task<NonWorkingDay> AddNonWorkingDayAsync(NonWorkingDay day)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        if (await db.NonWorkingDays.AnyAsync(n => n.BranchId == day.BranchId && n.DateLocal == day.DateLocal))
        {
            throw new InvalidOperationException("Non-working day already exists for this date.");
        }

        db.NonWorkingDays.Add(day);
        await db.SaveChangesAsync();
        return day;
    }

    public async Task<NonWorkingDayOverride> AddOverrideAsync(int branchId, DateOnly date, string reason, string approvedByUserId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        if (await db.NonWorkingDayOverrides.AnyAsync(o => o.BranchId == branchId && o.DateLocal == date && o.OverrideType == "OvertimeEnabled"))
        {
             throw new InvalidOperationException("Overtime override already exists for this date.");
        }

        // Create Audit Event first to get ID? Or commit together?
        // AuditService commits immediately.
        await _auditService.LogAsync(branchId, "OverrideCreated", "NonWorkingDayOverride", date.ToString(), approvedByUserId, reason, "OvertimeEnabled");

        // Fetch the audit event we just created? Or just rely on loose coupling.
        // The requirement says "AuditEventId (required)".
        // Let's fetch the latest audit for this user/branch/type to link it.
        var audit = await db.AuditEvents
            .Where(a => a.BranchId == branchId && a.ActorUserId == approvedByUserId && a.EventType == "OverrideCreated")
            .OrderByDescending(a => a.OccurredAtUtc)
            .FirstOrDefaultAsync();

        var overrideEntity = new NonWorkingDayOverride
        {
            BranchId = branchId,
            DateLocal = date,
            OverrideType = "OvertimeEnabled",
            Reason = reason,
            ApprovedByUserId = approvedByUserId,
            ApprovedAtUtc = DateTime.UtcNow,
            AuditEventId = audit?.Id ?? 0 // Should not happen if transactional, but services are separate here.
        };

        db.NonWorkingDayOverrides.Add(overrideEntity);
        await db.SaveChangesAsync();
        return overrideEntity;
    }

    public async Task<bool> IsOvertimeEnabledAsync(int branchId, DateOnly date)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.NonWorkingDayOverrides
            .AnyAsync(o => o.BranchId == branchId && o.DateLocal == date && o.OverrideType == "OvertimeEnabled");
    }
}
