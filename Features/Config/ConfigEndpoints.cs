using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using CMetalsFulfillment.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Features.Config
{
    public static class ConfigEndpoints
    {
        public static void MapConfigEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/admin").RequireAuthorization(AuthConstants.PolicyCanAdminBranch);

            // --- MACHINES ---
            group.MapGet("/machines", async (IBranchContext branchContext, ApplicationDbContext db) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");
                var list = await db.Machines
                    .Where(x => x.BranchId == branchContext.BranchId.Value)
                    .OrderBy(x => x.Name)
                    .ToListAsync();
                return Results.Ok(list);
            });

            group.MapPost("/machines", async (IBranchContext branchContext, ApplicationDbContext db, [FromBody] Machine machine) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                // Enforce branch scope
                machine.BranchId = branchContext.BranchId.Value;

                if (machine.MachineId == 0)
                {
                    db.Machines.Add(machine);
                }
                else
                {
                    var existing = await db.Machines.FirstOrDefaultAsync(x => x.MachineId == machine.MachineId && x.BranchId == machine.BranchId);
                    if (existing == null) return Results.NotFound();
                    existing.Name = machine.Name;
                    existing.MachineType = machine.MachineType;
                    existing.IsActive = machine.IsActive;
                }
                await db.SaveChangesAsync();
                return Results.Ok(machine);
            });

            // --- PICK/PACK STATIONS ---
            group.MapGet("/pickpackstations", async (IBranchContext branchContext, ApplicationDbContext db) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");
                var list = await db.PickPackStations
                    .Where(x => x.BranchId == branchContext.BranchId.Value)
                    .OrderBy(x => x.Name)
                    .ToListAsync();
                return Results.Ok(list);
            });

            group.MapPost("/pickpackstations", async (IBranchContext branchContext, ApplicationDbContext db, [FromBody] PickPackStation station) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                station.BranchId = branchContext.BranchId.Value;

                if (station.PickPackStationId == 0)
                {
                    db.PickPackStations.Add(station);
                }
                else
                {
                    var existing = await db.PickPackStations.FirstOrDefaultAsync(x => x.PickPackStationId == station.PickPackStationId && x.BranchId == station.BranchId);
                    if (existing == null) return Results.NotFound();
                    existing.Name = station.Name;
                    existing.IsActive = station.IsActive;
                }
                await db.SaveChangesAsync();
                return Results.Ok(station);
            });

            // --- SHIFTS ---
            group.MapGet("/shifts", async (IBranchContext branchContext, ApplicationDbContext db) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");
                var list = await db.ShiftTemplates
                    .Where(x => x.BranchId == branchContext.BranchId.Value)
                    .OrderBy(x => x.Name)
                    .ToListAsync();
                return Results.Ok(list);
            });

            group.MapPost("/shifts", async (IBranchContext branchContext, ApplicationDbContext db, [FromBody] ShiftTemplate shift) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                shift.BranchId = branchContext.BranchId.Value;

                if (shift.ShiftTemplateId == 0)
                {
                    db.ShiftTemplates.Add(shift);
                }
                else
                {
                    var existing = await db.ShiftTemplates.FirstOrDefaultAsync(x => x.ShiftTemplateId == shift.ShiftTemplateId && x.BranchId == shift.BranchId);
                    if (existing == null) return Results.NotFound();
                    existing.Name = shift.Name;
                    existing.StartTimeLocal = shift.StartTimeLocal;
                    existing.EndTimeLocal = shift.EndTimeLocal;
                    existing.BreakRulesJson = shift.BreakRulesJson;
                    existing.IsActive = shift.IsActive;
                }
                await db.SaveChangesAsync();
                return Results.Ok(shift);
            });
        }
    }
}
