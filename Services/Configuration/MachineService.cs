using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services.Configuration;

public class MachineService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IMachineService
{
    // Machine CRUD
    public async Task<List<Machine>> GetMachinesAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Machines.AsNoTracking().Where(m => m.BranchId == branchId).ToListAsync();
    }

    public async Task CreateMachineAsync(Machine machine)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        context.Machines.Add(machine);
        await context.SaveChangesAsync();
    }

    public async Task UpdateMachineAsync(Machine machine)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var existing = await context.Machines.FirstOrDefaultAsync(m => m.Id == machine.Id && m.BranchId == machine.BranchId);
        if (existing == null) throw new InvalidOperationException("Machine not found");

        context.Entry(existing).OriginalValues["Version"] = machine.Version;
        existing.Name = machine.Name;
        existing.Description = machine.Description;
        existing.Status = machine.Status;

        await context.SaveChangesAsync();
    }

    public async Task DeleteMachineAsync(int id, int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var machine = await context.Machines.FirstOrDefaultAsync(m => m.Id == id && m.BranchId == branchId);
        if (machine != null)
        {
            context.Machines.Remove(machine);
            await context.SaveChangesAsync();
        }
    }

    // Operator Logic
    public async Task AssignOperatorAsync(int machineId, string userId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var machine = await context.Machines.FindAsync(machineId);
        if (machine == null) throw new InvalidOperationException("Machine not found");

        // End active assignment for this user/machine? Or just append?
        // Typically operators work on one machine at a time.
        // Let's close any open assignment for this user on THIS machine first (or any machine? Usually any).
        // Requirement doesn't specify single-machine constraint, but let's assume one active assignment per user per branch.

        var activeAssignments = await context.MachineOperatorAssignments
            .Where(a => a.UserId == userId && a.BranchId == machine.BranchId && a.UnassignedAtUtc == null)
            .ToListAsync();

        foreach (var assignment in activeAssignments)
        {
            assignment.UnassignedAtUtc = DateTime.UtcNow;
        }

        context.MachineOperatorAssignments.Add(new MachineOperatorAssignment
        {
            BranchId = machine.BranchId,
            MachineId = machineId,
            UserId = userId,
            AssignedAtUtc = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }

    public async Task UnassignOperatorAsync(int machineId, string userId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var assignment = await context.MachineOperatorAssignments
            .Where(a => a.MachineId == machineId && a.UserId == userId && a.UnassignedAtUtc == null)
            .FirstOrDefaultAsync();

        if (assignment != null)
        {
            assignment.UnassignedAtUtc = DateTime.UtcNow;
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<MachineOperatorAssignment>> GetActiveAssignmentsAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.MachineOperatorAssignments
            .AsNoTracking()
            .Include(a => a.User)
            .Include(a => a.Machine)
            .Where(a => a.BranchId == branchId && a.UnassignedAtUtc == null)
            .ToListAsync();
    }

    // Station CRUD
    public async Task<List<PickPackStation>> GetStationsAsync(int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.PickPackStations.AsNoTracking().Where(s => s.BranchId == branchId).ToListAsync();
    }

    public async Task CreateStationAsync(PickPackStation station)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        context.PickPackStations.Add(station);
        await context.SaveChangesAsync();
    }

    public async Task UpdateStationAsync(PickPackStation station)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var existing = await context.PickPackStations.FirstOrDefaultAsync(s => s.Id == station.Id && s.BranchId == station.BranchId);
        if (existing == null) throw new InvalidOperationException("Station not found");

        context.Entry(existing).OriginalValues["Version"] = station.Version;
        existing.Name = station.Name;
        existing.Status = station.Status;

        await context.SaveChangesAsync();
    }

    public async Task DeleteStationAsync(int id, int branchId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();
        var station = await context.PickPackStations.FirstOrDefaultAsync(s => s.Id == id && s.BranchId == branchId);
        if (station != null)
        {
            context.PickPackStations.Remove(station);
            await context.SaveChangesAsync();
        }
    }
}
