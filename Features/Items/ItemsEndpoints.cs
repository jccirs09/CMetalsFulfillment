using CMetalsFulfillment.Data;
using CMetalsFulfillment.Domain;
using CMetalsFulfillment.Features.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;

namespace CMetalsFulfillment.Features.Items
{
    public static class ItemsEndpoints
    {
        public static void MapItemsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/items").RequireAuthorization("AdminPolicy");

            group.MapGet("/", async (ApplicationDbContext db, IBranchContext branchContext, [FromQuery] string? search) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                var query = db.Items
                    .Where(i => i.BranchId == branchId && i.IsActive)
                    .AsNoTracking();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    query = query.Where(i => i.ItemCode.ToLower().Contains(search) || i.Description.ToLower().Contains(search));
                }

                var items = await query.ToListAsync();
                return Results.Ok(items);
            });

            // PUT /api/items/{id}/ppsf
            group.MapPut("/{id}/ppsf", async (ApplicationDbContext db, IBranchContext branchContext, int id, [FromBody] UpdatePpsfDto dto) =>
            {
                var branchId = await branchContext.GetBranchIdAsync();
                var item = await db.Items.FirstOrDefaultAsync(i => i.Id == id && i.BranchId == branchId);
                if (item == null) return Results.NotFound();

                item.PoundsPerSquareFoot = dto.PoundsPerSquareFoot;
                await db.SaveChangesAsync();
                return Results.Ok(item);
            });

            // POST /api/items/import
            group.MapPost("/import", async (ApplicationDbContext db, IBranchContext branchContext, IFormFile file) =>
            {
                if (file == null || file.Length == 0) return Results.BadRequest("No file uploaded");

                using var stream = file.OpenReadStream();
                var rows = stream.Query<ItemImportDto>();

                int imported = 0;
                var branchId = await branchContext.GetBranchIdAsync();

                foreach (var row in rows)
                {
                    if (string.IsNullOrWhiteSpace(row.ItemCode)) continue;

                    var itemCode = row.ItemCode.Trim();
                    var description = row.Description?.Trim() ?? "";
                    var coilRel = row.CoilRelationship?.Trim();

                    // Derive UOM
                    string uom;
                    if (!string.IsNullOrEmpty(coilRel))
                    {
                        // Logic: If CoilRelationship is present, usually it implies this is a Child item (Sheet) or Parent (Coil).
                        // If logic "Prefer CoilRelationship if consistent" is vague, I will stick to the fallback rule more strictly if CoilRel is empty.
                        // But if CoilRel is NOT empty, what is the UOM?
                        // If it has a relationship, maybe it's PCS (Sheet) made from LBS (Coil)?
                        // The rule says "if empty, fallback to...". This implies if NOT empty, use it? But CoilRelationship is a string (e.g. parent item code). It's not "PCS" or "LBS".
                        // Maybe the prompt meant "Use CoilRelationship to determine"?
                        // "Derive UOM: Prefer CoilRelationship if consistent; if empty, fallback..."
                        // If CoilRelationship is populated, it usually means it's a SHEET item derived from a COIL. Sheets are PCS.
                        // Coils don't have CoilRelationship (or points to nothing). Coils are LBS.
                        // So: If CoilRel is NOT empty => PCS.
                        // Else => Fallback.
                        uom = "PCS";
                    }
                    else
                    {
                        // Fallback
                        var descUpper = description.ToUpperInvariant();
                        if (descUpper.Contains("SHEET") || descUpper.Contains("SHT"))
                        {
                            uom = "PCS";
                        }
                        else
                        {
                            uom = "LBS";
                        }
                    }

                    var existing = await db.Items.FirstOrDefaultAsync(i => i.BranchId == branchId && i.ItemCode == itemCode);
                    if (existing != null)
                    {
                        existing.Description = description;
                        existing.CoilRelationship = coilRel;
                        existing.UOM = uom;
                        existing.IsActive = true;
                        // PPSF preserved
                    }
                    else
                    {
                        var newItem = new Item
                        {
                            BranchId = branchId,
                            ItemCode = itemCode,
                            Description = description,
                            CoilRelationship = coilRel,
                            UOM = uom,
                            IsActive = true,
                            PoundsPerSquareFoot = null // Default null
                        };
                        db.Items.Add(newItem);
                    }
                    imported++;
                }

                await db.SaveChangesAsync();
                return Results.Ok(new { Count = imported });
            }).DisableAntiforgery(); // For file upload forms often needed
        }
    }

    public class ItemImportDto
    {
        public string? ItemCode { get; set; }
        public string? Description { get; set; }
        public string? CoilRelationship { get; set; }
    }

    public class UpdatePpsfDto
    {
        public decimal? PoundsPerSquareFoot { get; set; }
    }
}
