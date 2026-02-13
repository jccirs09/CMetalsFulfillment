using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using CMetalsFulfillment.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMetalsFulfillment.Features.PickingLists
{
    public static class PickingListEndpoints
    {
        public static void MapPickingListEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/pl").RequireAuthorization();

            // --- LIST ---
            group.MapGet("/", async (IBranchContext branchContext, ApplicationDbContext db) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                var list = await db.PickingLists
                    .AsNoTracking()
                    .Where(x => x.ImportBranchId == branchContext.BranchId.Value)
                    .OrderByDescending(x => x.CreatedAtUtc)
                    .Take(50)
                    .ToListAsync();
                return Results.Ok(list);
            });

            // --- DETAIL ---
            group.MapGet("/{uid}", async (Guid uid, IBranchContext branchContext, ApplicationDbContext db) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                var pl = await db.PickingLists
                    .Include(x => x.CreatedByUser)
                    .FirstOrDefaultAsync(x => x.PickingListUid == uid && x.ImportBranchId == branchContext.BranchId.Value);

                if (pl == null) return Results.NotFound();

                var lines = await db.PickingListLines
                    .Include(x => x.AssignedStation)
                    .Where(x => x.PickingListUid == uid)
                    .OrderBy(x => x.LineNumber)
                    .ToListAsync();

                return Results.Ok(new { Header = pl, Lines = lines });
            });

            // --- IMPORT (Reimport logic) ---
            group.MapPost("/import", async (IBranchContext branchContext, ApplicationDbContext db, ClaimsPrincipal user, [FromBody] ImportPlDto dto) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");
                var branchId = branchContext.BranchId.Value;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

                // Get Branch Settings for Default Station
                var settings = await db.BranchSettings.FirstOrDefaultAsync(x => x.BranchId == branchId);
                int? defaultStationId = settings?.DefaultPickPackStationId;

                // Validate: If any line is PickPack, default station must be set
                if (dto.Lines.Any(x => x.FulfillmentKind == "PickPack") && !defaultStationId.HasValue)
                {
                    return Results.BadRequest("Default Pick/Pack Station not configured for this branch. Cannot import PickPack lines.");
                }

                // Check existing PL
                var pl = await db.PickingLists.FirstOrDefaultAsync(x => x.ImportBranchId == branchId && x.PickingListNumber == dto.PickingListNumber);

                if (pl == null)
                {
                    // Create New
                    pl = new PickingList
                    {
                        PickingListUid = Guid.NewGuid(),
                        ImportBranchId = branchId,
                        PickingListNumber = dto.PickingListNumber,
                        ShipToName = dto.ShipToName,
                        ShipToAddress = dto.ShipToAddress,
                        ShipToCity = dto.ShipToCity,
                        ShipToState = dto.ShipToState,
                        ShipToZip = dto.ShipToZip,
                        FOBPoint = dto.FOBPoint,
                        ShipDateLocal = dto.ShipDateLocal,
                        Status = "Open",
                        CreatedByUserId = userId,
                        CreatedAtUtc = DateTime.UtcNow
                    };
                    db.PickingLists.Add(pl);

                    // Add Lines
                    foreach (var lineDto in dto.Lines)
                    {
                        var line = new PickingListLine
                        {
                            PickingListUid = pl.PickingListUid,
                            LineNumber = lineDto.LineNumber,
                            ItemCode = lineDto.ItemCode,
                            QtyOrdered = lineDto.QtyOrdered,
                            UOM = lineDto.UOM,
                            FulfillmentKind = lineDto.FulfillmentKind,
                            Status = "Open",
                            AssignedPickPackStationId = lineDto.FulfillmentKind == "PickPack" ? defaultStationId : null
                        };
                        db.PickingListLines.Add(line);
                    }
                }
                else
                {
                    // Update Header
                    pl.ShipToName = dto.ShipToName; // etc...
                    // Reimport Logic for Lines
                    var existingLines = await db.PickingListLines.Where(x => x.PickingListUid == pl.PickingListUid).ToListAsync();

                    foreach (var lineDto in dto.Lines)
                    {
                        var existingLine = existingLines.FirstOrDefault(x => x.LineNumber == lineDto.LineNumber && x.ItemCode == lineDto.ItemCode);
                        if (existingLine == null)
                        {
                            // New Line
                            db.PickingListLines.Add(new PickingListLine
                            {
                                PickingListUid = pl.PickingListUid,
                                LineNumber = lineDto.LineNumber,
                                ItemCode = lineDto.ItemCode,
                                QtyOrdered = lineDto.QtyOrdered,
                                UOM = lineDto.UOM,
                                FulfillmentKind = lineDto.FulfillmentKind,
                                Status = "Open",
                                AssignedPickPackStationId = lineDto.FulfillmentKind == "PickPack" ? defaultStationId : null
                            });
                        }
                        else
                        {
                            // Update Line (if open)
                            if (existingLine.Status == "Open")
                            {
                                existingLine.QtyOrdered = lineDto.QtyOrdered;
                                existingLine.FulfillmentKind = lineDto.FulfillmentKind;
                                // Should we re-assign station? Maybe not if manually changed.
                            }
                        }
                    }

                    // Missing Lines -> PendingCancel
                    var importedLineKeys = dto.Lines.Select(x => x.LineNumber + "-" + x.ItemCode).ToHashSet();
                    foreach (var oldLine in existingLines)
                    {
                        if (!importedLineKeys.Contains(oldLine.LineNumber + "-" + oldLine.ItemCode))
                        {
                            if (oldLine.Status == "Open")
                            {
                                oldLine.Status = "PendingCancel";
                                db.PickingListEvents.Add(new PickingListEvent
                                {
                                    PickingListUid = pl.PickingListUid,
                                    UserId = userId,
                                    EventType = "LinePendingCancel",
                                    Details = $"Line {oldLine.LineNumber} missing in reimport."
                                });
                            }
                        }
                    }
                }

                await db.SaveChangesAsync();
                return Results.Ok(pl.PickingListUid);
            });

            // --- CONFIRM CANCEL ---
            group.MapPost("/{uid}/lines/{lineId}/confirm-cancel", async (Guid uid, int lineId, IBranchContext branchContext, ApplicationDbContext db, ClaimsPrincipal user) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                var line = await db.PickingListLines.FirstOrDefaultAsync(x => x.PickingListUid == uid && x.Id == lineId);
                if (line == null) return Results.NotFound();

                if (line.Status == "PendingCancel")
                {
                    line.Status = "Cancelled";
                    db.PickingListEvents.Add(new PickingListEvent
                    {
                        PickingListUid = uid,
                        UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System",
                        EventType = "LineCancelled",
                        Details = $"Line {line.LineNumber} cancelled by planner."
                    });
                    await db.SaveChangesAsync();
                }
                return Results.Ok();
            });
        }
    }

    public class ImportPlDto
    {
        public string PickingListNumber { get; set; } = string.Empty;
        public string? ShipToName { get; set; }
        public string? ShipToAddress { get; set; }
        public string? ShipToCity { get; set; }
        public string? ShipToState { get; set; }
        public string? ShipToZip { get; set; }
        public string? FOBPoint { get; set; }
        public DateTime? ShipDateLocal { get; set; }
        public List<ImportPlLineDto> Lines { get; set; } = new();
    }

    public class ImportPlLineDto
    {
        public int LineNumber { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public decimal QtyOrdered { get; set; }
        public string UOM { get; set; } = "PCS";
        public string FulfillmentKind { get; set; } = "PickPack";
    }
}
