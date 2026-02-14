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
            var group = app.MapGroup("/api/config").RequireAuthorization("AdminPolicy");

            // Machines
            group.MapGet("/machines", async (ApplicationDbContext db, IBranchContext branchContext) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                var machines = await db.Machines
                    .Where(m => m.BranchId == branchId)
                    .AsNoTracking()
                    .ToListAsync();
                return Results.Ok(machines);
            });

            group.MapPost("/machines", async (ApplicationDbContext db, IBranchContext branchContext, [FromBody] Machine machine) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                machine.BranchId = branchId;
                db.Machines.Add(machine);
                await db.SaveChangesAsync();
                return Results.Ok(machine);
            });

            group.MapPut("/machines/{id}", async (ApplicationDbContext db, IBranchContext branchContext, int id, [FromBody] Machine machine) =>
            {
                if (id != machine.MachineId) return Results.BadRequest();
                var branchId = await branchContext.GetBranchIdAsync();
                var existing = await db.Machines.FirstOrDefaultAsync(m => m.MachineId == id && m.BranchId == branchId);
                if (existing == null) return Results.NotFound();

                existing.Name = machine.Name;
                existing.MachineType = machine.MachineType;
                existing.IsActive = machine.IsActive;

                await db.SaveChangesAsync();
                return Results.Ok(existing);
            });

            // PickPack Stations
            group.MapGet("/stations", async (ApplicationDbContext db, IBranchContext branchContext) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                var stations = await db.PickPackStations
                    .Where(p => p.BranchId == branchId)
                    .AsNoTracking()
                    .ToListAsync();
                return Results.Ok(stations);
            });

            group.MapPost("/stations", async (ApplicationDbContext db, IBranchContext branchContext, [FromBody] PickPackStation station) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                station.BranchId = branchId;
                db.PickPackStations.Add(station);
                await db.SaveChangesAsync();
                return Results.Ok(station);
            });

            group.MapPut("/stations/{id}", async (ApplicationDbContext db, IBranchContext branchContext, int id, [FromBody] PickPackStation station) =>
            {
                if (id != station.PickPackStationId) return Results.BadRequest();
                var branchId = await branchContext.GetBranchIdAsync();
                var existing = await db.PickPackStations.FirstOrDefaultAsync(p => p.PickPackStationId == id && p.BranchId == branchId);
                if (existing == null) return Results.NotFound();

                existing.Name = station.Name;
                existing.IsActive = station.IsActive;

                await db.SaveChangesAsync();
                return Results.Ok(existing);
            });

            // Shift Templates
            group.MapGet("/shifts", async (ApplicationDbContext db, IBranchContext branchContext) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                var shifts = await db.ShiftTemplates
                    .Where(s => s.BranchId == branchId)
                    .AsNoTracking()
                    .ToListAsync();
                return Results.Ok(shifts);
            });

            group.MapPost("/shifts", async (ApplicationDbContext db, IBranchContext branchContext, [FromBody] ShiftTemplate shift) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                shift.BranchId = branchId;
                db.ShiftTemplates.Add(shift);
                await db.SaveChangesAsync();
                return Results.Ok(shift);
            });

            group.MapPut("/shifts/{id}", async (ApplicationDbContext db, IBranchContext branchContext, int id, [FromBody] ShiftTemplate shift) =>
            {
                if (id != shift.ShiftTemplateId) return Results.BadRequest();
                var branchId = await branchContext.GetBranchIdAsync();
                var existing = await db.ShiftTemplates.FirstOrDefaultAsync(s => s.ShiftTemplateId == id && s.BranchId == branchId);
                if (existing == null) return Results.NotFound();

                existing.Name = shift.Name;
                existing.StartTimeLocal = shift.StartTimeLocal;
                existing.EndTimeLocal = shift.EndTimeLocal;
                existing.BreakRulesJson = shift.BreakRulesJson;
                existing.IsActive = shift.IsActive;

                await db.SaveChangesAsync();
                return Results.Ok(existing);
            });

            // Shipping Regions
            group.MapGet("/shipping/regions", async (ApplicationDbContext db, IBranchContext branchContext) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                var regions = await db.ShippingRegions
                    .Where(r => r.BranchId == branchId)
                    .AsNoTracking()
                    .ToListAsync();
                return Results.Ok(regions);
            });

            group.MapPost("/shipping/regions", async (ApplicationDbContext db, IBranchContext branchContext, [FromBody] ShippingRegion region) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                region.BranchId = branchId;
                db.ShippingRegions.Add(region);
                await db.SaveChangesAsync();
                return Results.Ok(region);
            });

            group.MapPut("/shipping/regions/{id}", async (ApplicationDbContext db, IBranchContext branchContext, int id, [FromBody] ShippingRegion region) =>
            {
                if (id != region.Id) return Results.BadRequest();
                var branchId = await branchContext.GetBranchIdAsync();
                var existing = await db.ShippingRegions.FirstOrDefaultAsync(r => r.Id == id && r.BranchId == branchId);
                if (existing == null) return Results.NotFound();

                existing.Name = region.Name;
                existing.IsActive = region.IsActive;
                await db.SaveChangesAsync();
                return Results.Ok(existing);
            });

            // Shipping Groups
            group.MapGet("/shipping/groups", async (ApplicationDbContext db, IBranchContext branchContext) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                var groups = await db.ShippingGroups
                    .Where(g => g.BranchId == branchId)
                    .Include(g => g.ShippingRegion)
                    .AsNoTracking()
                    .ToListAsync();
                return Results.Ok(groups);
            });

            group.MapPost("/shipping/groups", async (ApplicationDbContext db, IBranchContext branchContext, [FromBody] ShippingGroup group) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                group.BranchId = branchId;
                // Verify Region belongs to branch
                var region = await db.ShippingRegions.FirstOrDefaultAsync(r => r.Id == group.ShippingRegionId && r.BranchId == branchId);
                if (region == null) return Results.BadRequest("Invalid Region");

                db.ShippingGroups.Add(group);
                await db.SaveChangesAsync();
                return Results.Ok(group);
            });

            group.MapPut("/shipping/groups/{id}", async (ApplicationDbContext db, IBranchContext branchContext, int id, [FromBody] ShippingGroup group) =>
            {
                if (id != group.Id) return Results.BadRequest();
                var branchId = await branchContext.GetBranchIdAsync();
                var existing = await db.ShippingGroups.FirstOrDefaultAsync(g => g.Id == id && g.BranchId == branchId);
                if (existing == null) return Results.NotFound();

                // Verify Region belongs to branch
                var region = await db.ShippingRegions.FirstOrDefaultAsync(r => r.Id == group.ShippingRegionId && r.BranchId == branchId);
                if (region == null) return Results.BadRequest("Invalid Region");

                existing.Name = group.Name;
                existing.ShippingRegionId = group.ShippingRegionId;
                existing.IsActive = group.IsActive;
                await db.SaveChangesAsync();
                return Results.Ok(existing);
            });

            // Shipping FSA Rules
            group.MapGet("/shipping/rules", async (ApplicationDbContext db, IBranchContext branchContext) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                var rules = await db.ShippingFsaRules
                    .Where(r => r.BranchId == branchId)
                    .Include(r => r.ShippingRegion)
                    .Include(r => r.ShippingGroup)
                    .OrderBy(r => r.FsaPrefix).ThenByDescending(r => r.Priority)
                    .AsNoTracking()
                    .ToListAsync();
                return Results.Ok(rules);
            });

            group.MapPost("/shipping/rules", async (ApplicationDbContext db, IBranchContext branchContext, [FromBody] ShippingFsaRule rule) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                rule.BranchId = branchId;
                // Verify Region/Group if set
                if (rule.ShippingRegionId.HasValue)
                {
                    var region = await db.ShippingRegions.AnyAsync(r => r.Id == rule.ShippingRegionId && r.BranchId == branchId);
                    if (!region) return Results.BadRequest("Invalid Region");
                }
                if (rule.ShippingGroupId.HasValue)
                {
                    var group = await db.ShippingGroups.AnyAsync(g => g.Id == rule.ShippingGroupId && g.BranchId == branchId);
                    if (!group) return Results.BadRequest("Invalid Group");
                }

                db.ShippingFsaRules.Add(rule);
                await db.SaveChangesAsync();
                return Results.Ok(rule);
            });

            group.MapPut("/shipping/rules/{id}", async (ApplicationDbContext db, IBranchContext branchContext, int id, [FromBody] ShippingFsaRule rule) =>
            {
                if (id != rule.Id) return Results.BadRequest();
                var branchId = await branchContext.GetBranchIdAsync();
                var existing = await db.ShippingFsaRules.FirstOrDefaultAsync(r => r.Id == id && r.BranchId == branchId);
                if (existing == null) return Results.NotFound();

                if (rule.ShippingRegionId.HasValue)
                {
                    var region = await db.ShippingRegions.AnyAsync(r => r.Id == rule.ShippingRegionId && r.BranchId == branchId);
                    if (!region) return Results.BadRequest("Invalid Region");
                }
                if (rule.ShippingGroupId.HasValue)
                {
                    var group = await db.ShippingGroups.AnyAsync(g => g.Id == rule.ShippingGroupId && g.BranchId == branchId);
                    if (!group) return Results.BadRequest("Invalid Group");
                }

                existing.FsaPrefix = rule.FsaPrefix;
                existing.ShippingRegionId = rule.ShippingRegionId;
                existing.ShippingGroupId = rule.ShippingGroupId;
                existing.Priority = rule.Priority;
                existing.IsActive = rule.IsActive;
                await db.SaveChangesAsync();
                return Results.Ok(existing);
            });
        }
    }
}
