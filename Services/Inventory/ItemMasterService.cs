using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;

namespace CMetalsFulfillment.Services.Inventory;

public class ItemMasterService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IItemMasterService
{
    public async Task ImportItemMasterAsync(Stream excelStream, int branchId)
    {
        var rows = excelStream.Query().ToList(); // Read all rows as dynamic objects

        using var context = await dbContextFactory.CreateDbContextAsync();

        foreach (IDictionary<string, object> row in rows)
        {
            var itemCode = GetValue(row, "ItemCode");
            var description = GetValue(row, "Description");
            var coilRelationship = GetValue(row, "CoilRelationship");

            if (string.IsNullOrWhiteSpace(itemCode)) continue;

            string uom = "LBS"; // Default per rule

            if (!string.IsNullOrWhiteSpace(coilRelationship))
            {
                if (coilRelationship.Contains("sheet", StringComparison.OrdinalIgnoreCase))
                {
                    uom = "PCS";
                }
                // Else LBS (default)
            }
            else // CoilRelationship is blank
            {
                if (description.Contains("SHEET", StringComparison.OrdinalIgnoreCase) ||
                    description.Contains("SHT", StringComparison.OrdinalIgnoreCase))
                {
                    uom = "PCS";
                }
                // Else LBS (default)
            }

            var existing = await context.ItemMasters
                .FirstOrDefaultAsync(x => x.BranchId == branchId && x.ItemCode == itemCode);

            if (existing != null)
            {
                // Update
                existing.Description = description;
                existing.CoilRelationship = string.IsNullOrWhiteSpace(coilRelationship) ? null : coilRelationship;
                existing.Uom = uom;
                // Preserve Ppsf
            }
            else
            {
                // Insert
                context.ItemMasters.Add(new ItemMaster
                {
                    BranchId = branchId,
                    ItemCode = itemCode,
                    Description = description,
                    CoilRelationship = string.IsNullOrWhiteSpace(coilRelationship) ? null : coilRelationship,
                    Ppsf = null,
                    Uom = uom
                });
            }
        }

        await context.SaveChangesAsync();
    }

    private string GetValue(IDictionary<string, object> row, string key)
    {
        if (row.TryGetValue(key, out var value) && value != null)
        {
            return value.ToString()?.Trim() ?? string.Empty;
        }
        return string.Empty;
    }
}
