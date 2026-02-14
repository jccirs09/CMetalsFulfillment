using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class MachineService : IMachineService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public MachineService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<Machine>> GetMachinesAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Machines.Where(m => m.BranchId == branchId).ToListAsync();
    }

    public async Task<Machine?> GetMachineAsync(int id, int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Machines.FirstOrDefaultAsync(m => m.Id == id && m.BranchId == branchId);
    }

    public async Task<Machine> CreateMachineAsync(Machine machine)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        // Basic validation
        if (await db.Machines.AnyAsync(m => m.BranchId == machine.BranchId && m.MachineCode == machine.MachineCode))
        {
            throw new InvalidOperationException("Machine Code already exists for this branch.");
        }

        db.Machines.Add(machine);
        await db.SaveChangesAsync();
        return machine;
    }

    public async Task<Machine> UpdateMachineAsync(Machine machine)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var existing = await db.Machines.FirstOrDefaultAsync(m => m.Id == machine.Id && m.BranchId == machine.BranchId);
        if (existing == null) throw new KeyNotFoundException("Machine not found.");

        existing.MachineName = machine.MachineName;
        existing.MachineType = machine.MachineType;
        existing.DefaultDurationMinutes = machine.DefaultDurationMinutes;
        existing.IsActive = machine.IsActive;
        existing.Notes = machine.Notes;
        existing.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return existing;
    }

    public async Task AddOperatorAsync(int machineId, int branchId, string userId, string assignedBy)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        if (await db.MachineOperatorAssignments.AnyAsync(m => m.BranchId == branchId && m.MachineId == machineId && m.UserId == userId))
        {
            return;
        }

        db.MachineOperatorAssignments.Add(new MachineOperatorAssignment
        {
            BranchId = branchId,
            MachineId = machineId,
            UserId = userId,
            IsQualified = true,
            QualifiedAtUtc = DateTime.UtcNow,
            QualifiedByUserId = assignedBy
        });
        await db.SaveChangesAsync();
    }

    public async Task RemoveOperatorAsync(int machineId, int branchId, string userId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var assignment = await db.MachineOperatorAssignments
            .FirstOrDefaultAsync(m => m.BranchId == branchId && m.MachineId == machineId && m.UserId == userId);

        if (assignment != null)
        {
            db.MachineOperatorAssignments.Remove(assignment);
            await db.SaveChangesAsync();
        }
    }

    public async Task<List<MachineOperatorAssignment>> GetOperatorsAsync(int machineId, int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.MachineOperatorAssignments
            .Include(a => a.User)
            .Where(m => m.BranchId == branchId && m.MachineId == machineId)
            .ToListAsync();
    }
}
