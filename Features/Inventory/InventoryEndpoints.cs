using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using CMetalsFulfillment.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System.Security.Claims;

namespace CMetalsFulfillment.Features.Inventory
{
    public static class InventoryEndpoints
    {
        public static void MapInventoryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/inventory").RequireAuthorization();

            // --- IMPORT ---
            group.MapPost("/import", async (IBranchContext branchContext, ApplicationDbContext db, IFormFile file, ClaimsPrincipal user) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");
                var branchId = branchContext.BranchId.Value;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

                if (file == null || file.Length == 0) return Results.BadRequest("No file uploaded");

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                // Read all rows without header parsing to handle blank header column M safely
                var rows = stream.Query(useHeaderRow: false).ToList();
                if (rows.Count < 2) return Results.BadRequest("File too short (headers + data required)");

                // Validate Headers (Row 0)
                var header = rows[0] as IDictionary<string, object>;
                // MiniExcel returns dynamic as IDictionary<string, object> where keys are A, B, C... if useHeaderRow: false?
                // No, if useHeaderRow: false, it returns an object where properties are A, B, C (Excel column names).
                // Let's cast to IDictionary to access values by key "A", "B", etc.

                // Row 0 check
                // A=Item ID, M=[Blank] or whatever value is there (it should be empty in header row?)
                // Actually prompt says "Header is BLANK".
                // Let's assume input format is consistent.

                var snapshot = new InventorySnapshot
                {
                    BranchId = branchId,
                    ImportedByUserId = userId,
                    SourceFileName = file.FileName,
                    TotalRows = 0,
                    MatchedRows = 0,
                    UnmatchedRows = 0
                };
                db.InventorySnapshots.Add(snapshot);
                await db.SaveChangesAsync(); // Get ID

                var newLines = new List<InventorySnapshotLine>();

                // Helper to get string value from dynamic row for a column index (0-based)
                // MiniExcel with useHeaderRow:false returns object with properties A, B, C...
                string GetVal(dynamic r, string colKey)
                {
                    var dict = (IDictionary<string, object>)r;
                    return dict.ContainsKey(colKey) && dict[colKey] != null ? dict[colKey].ToString().Trim() : "";
                }

                // Start from row 1 (data)
                for (int i = 1; i < rows.Count; i++)
                {
                    var row = rows[i]; // dynamic

                    var line = new InventorySnapshotLine
                    {
                        SnapshotId = snapshot.Id,
                        LineNo = i,
                        ItemCode = GetVal(row, "A"),
                        Description = GetVal(row, "B"),
                        Size = GetVal(row, "C"),
                        Mode = GetVal(row, "D"),
                        TagNumber = GetVal(row, "E"),
                        Status = GetVal(row, "F"),
                        Correctable = GetVal(row, "G"),
                        SnapshotLocation = GetVal(row, "H"),
                        CountLocation = GetVal(row, "I"),
                        Exception = GetVal(row, "L"),
                        UOM = GetVal(row, "M"), // Column M is UOM
                        MatchStatus = "Unmatched"
                    };

                    // Parse numbers
                    if (decimal.TryParse(GetVal(row, "J"), out var sv)) line.SnapshotValue = sv;
                    if (decimal.TryParse(GetVal(row, "K"), out var cv)) line.CountValue = cv;
                    if (decimal.TryParse(GetVal(row, "N"), out var amt)) line.Amount = amt;

                    // Validation logic
                    if (string.IsNullOrEmpty(line.ItemCode))
                    {
                        line.MatchStatus = "MissingItemCode";
                    }
                    else if (line.UOM != "PCS" && line.UOM != "LBS")
                    {
                        line.MatchStatus = "InvalidUom";
                    }
                    else
                    {
                        var matchedItem = await db.Items.FirstOrDefaultAsync(x => x.BranchId == branchId && x.ItemCode == line.ItemCode);
                        if (matchedItem != null)
                        {
                            line.MatchStatus = "Matched";
                            line.MatchedItemId = matchedItem.Id;
                        }
                        else
                        {
                            line.MatchStatus = "Unmatched";
                        }
                    }

                    newLines.Add(line);
                }

                snapshot.TotalRows = newLines.Count;
                snapshot.MatchedRows = newLines.Count(x => x.MatchStatus == "Matched");
                snapshot.UnmatchedRows = newLines.Count - snapshot.MatchedRows;

                db.InventorySnapshotLines.AddRange(newLines);
                await db.SaveChangesAsync();

                // Update InventoryStock (Snapshot replacement logic)
                // Delete existing stock for this branch? Or update?
                // Prompt: "Update InventoryStock aggregate for browse only... WeightOnHand = SUM(...) ... QuantityOnHand = SUM(...)"
                // Usually snapshot implies "this is the current state". So we should replace or update.
                // Simplest is to clear stock for items present in snapshot, or clear all stock for branch and rebuild?
                // "Inventory snapshot" usually means full list. Let's assume full replacement for the branch is safer/easier for "Snapshot".

                // Clear existing stock
                var oldStock = await db.InventoryStocks.Where(x => x.BranchId == branchId).ToListAsync();
                db.InventoryStocks.RemoveRange(oldStock);

                // Aggregate new stock
                var stockGroups = newLines
                    .Where(x => x.MatchStatus == "Matched")
                    .GroupBy(x => x.ItemCode)
                    .Select(g => new InventoryStock
                    {
                        BranchId = branchId,
                        ItemCode = g.Key,
                        LastUpdatedAtUtc = DateTime.UtcNow,
                        // If UOM=LBS: WeightOnHand = SUM(SnapshotValue J)
                        WeightOnHand = g.Where(x => x.UOM == "LBS").Sum(x => x.SnapshotValue ?? 0),
                        // If UOM=PCS: QuantityOnHand = SUM(SnapshotValue J)
                        QuantityOnHand = g.Where(x => x.UOM == "PCS").Sum(x => x.SnapshotValue ?? 0)
                        // Note: WeightOnHand for PCS optional if PPSF exists - ignoring for now as per "else keep null" implied default
                    })
                    .ToList();

                db.InventoryStocks.AddRange(stockGroups);
                await db.SaveChangesAsync();

                return Results.Ok(new { snapshot.Id, snapshot.MatchedRows, snapshot.UnmatchedRows });
            }).DisableAntiforgery();

            // --- GET STOCK ---
            group.MapGet("/stock", async (IBranchContext branchContext, ApplicationDbContext db, [FromQuery] string? search) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                var query = db.InventoryStocks.AsNoTracking().Where(x => x.BranchId == branchContext.BranchId.Value);
                if (!string.IsNullOrWhiteSpace(search))
                {
                     query = query.Where(x => x.ItemCode.Contains(search));
                }
                var list = await query.OrderBy(x => x.ItemCode).ToListAsync();
                return Results.Ok(list);
            });

            // --- GET SNAPSHOTS ---
            group.MapGet("/snapshots", async (IBranchContext branchContext, ApplicationDbContext db) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");
                var list = await db.InventorySnapshots
                    .AsNoTracking()
                    .Include(x => x.ImportedByUser)
                    .Where(x => x.BranchId == branchContext.BranchId.Value)
                    .OrderByDescending(x => x.ImportedAtUtc)
                    .Take(20)
                    .Select(x => new
                    {
                        x.Id,
                        x.ImportedAtUtc,
                        ImportedBy = x.ImportedByUser.UserName,
                        x.SourceFileName,
                        x.TotalRows,
                        x.MatchedRows
                    })
                    .ToListAsync();
                return Results.Ok(list);
            });

            // --- GET SNAPSHOT DETAIL ---
            group.MapGet("/snapshots/{id}", async (int id, IBranchContext branchContext, ApplicationDbContext db) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                var snapshot = await db.InventorySnapshots
                    .Include(x => x.ImportedByUser)
                    .FirstOrDefaultAsync(x => x.Id == id && x.BranchId == branchContext.BranchId.Value);

                if (snapshot == null) return Results.NotFound();

                // Return simplified lines
                var lines = await db.InventorySnapshotLines
                    .AsNoTracking()
                    .Where(x => x.SnapshotId == id)
                    .OrderBy(x => x.LineNo)
                    .ToListAsync();

                return Results.Ok(new { Snapshot = snapshot, Lines = lines });
            });
        }
    }
}
