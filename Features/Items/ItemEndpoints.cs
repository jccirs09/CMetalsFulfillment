using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using CMetalsFulfillment.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System.ComponentModel.DataAnnotations;

namespace CMetalsFulfillment.Features.Items
{
    public static class ItemEndpoints
    {
        public static void MapItemEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/items").RequireAuthorization();

            // --- LIST ---
            group.MapGet("/", async (IBranchContext branchContext, ApplicationDbContext db, [FromQuery] string? search) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                var query = db.Items
                    .AsNoTracking()
                    .Where(x => x.BranchId == branchContext.BranchId.Value);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.Trim();
                    query = query.Where(x => x.ItemCode.Contains(search) || (x.Description != null && x.Description.Contains(search)));
                }

                var list = await query.OrderBy(x => x.ItemCode).Take(100).ToListAsync();
                return Results.Ok(list);
            });

            // --- IMPORT (ADMIN) ---
            group.MapPost("/import", async (IBranchContext branchContext, ApplicationDbContext db, IFormFile file) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");
                var branchId = branchContext.BranchId.Value;

                if (file == null || file.Length == 0) return Results.BadRequest("No file uploaded");

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                var rows = stream.Query<ItemImportDto>().ToList();
                if (!rows.Any()) return Results.BadRequest("Empty file");

                // Process rows
                int processed = 0;
                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.ItemCode)) continue;

                    // Derive UOM
                    string uom = "LBS";
                    bool isSheet = false;

                    // Logic: Prefer CoilRelationship if consistent (meaning populated implies PCS)
                    if (!string.IsNullOrWhiteSpace(row.CoilRelationship))
                    {
                        uom = "PCS";
                        isSheet = true;
                    }
                    else
                    {
                        // Fallback to description
                        if (!string.IsNullOrWhiteSpace(row.Description))
                        {
                            var desc = row.Description.ToUpperInvariant();
                            if (desc.Contains("SHEET") || desc.Contains("SHT"))
                            {
                                uom = "PCS";
                                isSheet = true;
                            }
                        }
                    }

                    var item = await db.Items.FirstOrDefaultAsync(x => x.BranchId == branchId && x.ItemCode == row.ItemCode);
                    if (item == null)
                    {
                        item = new Item
                        {
                            BranchId = branchId,
                            ItemCode = row.ItemCode,
                            Description = row.Description,
                            CoilRelationship = row.CoilRelationship,
                            UOM = uom,
                            IsActive = true
                        };
                        db.Items.Add(item);
                    }
                    else
                    {
                        item.Description = row.Description;
                        item.CoilRelationship = row.CoilRelationship;
                        // UOM logic: usually we don't overwrite UOM if it was manually fixed, but prompt says "Derive UOM".
                        // Let's re-derive it to be consistent with import logic, unless it was edited?
                        // Prompt: "SysAdmin sets [PPSF] later via UI edit (not overwritten by import)".
                        // It doesn't say UOM is not overwritten. Assuming re-import updates UOM based on rules.
                        item.UOM = uom;
                        item.IsActive = true;
                    }
                    processed++;
                }

                await db.SaveChangesAsync();
                return Results.Ok(new { Processed = processed });
            }).DisableAntiforgery(); // Required for file upload in some scenarios, though standard post might be fine.

            // --- UPDATE PPSF ---
            group.MapPut("/{id}/ppsf", async (int id, [FromBody] UpdatePpsfRequest request, IBranchContext branchContext, ApplicationDbContext db) =>
            {
                if (!branchContext.BranchId.HasValue) return Results.BadRequest("Branch required");

                var item = await db.Items.FirstOrDefaultAsync(x => x.Id == id && x.BranchId == branchContext.BranchId.Value);
                if (item == null) return Results.NotFound();

                item.PoundsPerSquareFoot = request.PoundsPerSquareFoot;
                await db.SaveChangesAsync();
                return Results.Ok();
            });
        }
    }

    public class ItemImportDto
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string CoilRelationship { get; set; }
    }

    public class UpdatePpsfRequest
    {
        public decimal? PoundsPerSquareFoot { get; set; }
    }
}
