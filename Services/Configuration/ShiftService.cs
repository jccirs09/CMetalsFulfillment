using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services.Configuration;

public class ShiftService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IRoleResolver roleResolver) : IShiftService
{
    public async Task<List<ShiftTemplate>> GetShiftsAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.ShiftTemplates
            .AsNoTracking()
            .Where(s => s.BranchId == branchId)
            .OrderBy(s => s.StartTime)
            .ToListAsync();
    }

    public async Task<ShiftTemplate?> GetShiftAsync(int id, int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.ShiftTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id && s.BranchId == branchId);
    }

    public async Task CreateShiftAsync(ShiftTemplate shift, string userId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();

        // Enforce Rules
        shift.IsOvernight = shift.EndTime < shift.StartTime;

        if (shift.IsOvertime)
        {
            var hasRole = await roleResolver.HasRoleAsync(userId, shift.BranchId, "Supervisor") ||
                          await roleResolver.HasRoleAsync(userId, shift.BranchId, "BranchAdmin") ||
                          await roleResolver.HasRoleAsync(userId, shift.BranchId, "SystemAdmin");

            if (!hasRole)
            {
                throw new UnauthorizedAccessException("Only Supervisors can enable Overtime.");
            }

            context.AuditEvents.Add(new AuditEvent
            {
                BranchId = shift.BranchId,
                EventType = "ShiftConfig",
                EntityType = "ShiftTemplate",
                EntityId = "New",
                OccurredAtUtc = DateTime.UtcNow,
                ActorUserId = userId,
                Reason = "Initial Create with Overtime",
                PayloadJson = $"Shift {shift.Name} created with OT enabled."
            });
        }

        context.ShiftTemplates.Add(shift);
        await context.SaveChangesAsync();
    }

    public async Task UpdateShiftAsync(ShiftTemplate shift, string userId, string? overtimeReason = null)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var existing = await context.ShiftTemplates.FirstOrDefaultAsync(s => s.Id == shift.Id && s.BranchId == shift.BranchId);

        if (existing == null) throw new InvalidOperationException("Shift not found.");

        // Check Roles for OT
        if (shift.IsOvertime && !existing.IsOvertime)
        {
            var hasRole = await roleResolver.HasRoleAsync(userId, shift.BranchId, "Supervisor") ||
                          await roleResolver.HasRoleAsync(userId, shift.BranchId, "BranchAdmin") ||
                          await roleResolver.HasRoleAsync(userId, shift.BranchId, "SystemAdmin");

            if (!hasRole)
            {
                throw new UnauthorizedAccessException("Only Supervisors can enable Overtime.");
            }

            if (string.IsNullOrWhiteSpace(overtimeReason))
            {
                throw new ArgumentException("Reason is required to enable Overtime.");
            }

            context.AuditEvents.Add(new AuditEvent
            {
                BranchId = shift.BranchId,
                EventType = "ShiftConfig",
                EntityType = "ShiftTemplate",
                EntityId = shift.Id.ToString(),
                OccurredAtUtc = DateTime.UtcNow,
                ActorUserId = userId,
                Reason = overtimeReason,
                PayloadJson = $"Overtime Enabled for {shift.Name}"
            });
        }
        else if (!shift.IsOvertime && existing.IsOvertime)
        {
             context.AuditEvents.Add(new AuditEvent
             {
                 BranchId = shift.BranchId,
                 EventType = "ShiftConfig",
                 EntityType = "ShiftTemplate",
                 EntityId = shift.Id.ToString(),
                 OccurredAtUtc = DateTime.UtcNow,
                 ActorUserId = userId,
                 Reason = "Overtime Disabled",
                 PayloadJson = $"Overtime Disabled for {shift.Name}"
             });
        }

        // Set original version to ensure concurrency check happens against what user saw
        context.Entry(existing).OriginalValues["Version"] = shift.Version;

        // Update fields
        existing.Name = shift.Name;
        existing.StartTime = shift.StartTime;
        existing.EndTime = shift.EndTime;
        existing.IsOvernight = shift.EndTime < shift.StartTime;
        existing.IsOvertime = shift.IsOvertime;

        await context.SaveChangesAsync();
    }

    public async Task DeleteShiftAsync(int id, int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var shift = await context.ShiftTemplates.FirstOrDefaultAsync(s => s.Id == id && s.BranchId == branchId);
        if (shift != null)
        {
            context.ShiftTemplates.Remove(shift);
            await context.SaveChangesAsync();
        }
    }
}
