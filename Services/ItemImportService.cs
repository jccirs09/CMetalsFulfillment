using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;

namespace CMetalsFulfillment.Services;

public class ItemImportService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public ItemImportService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task ImportAsync(Stream stream, int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        // Load existing items into a dictionary for quick lookup
        var existingItems = await db.ItemMasters
            .Where(x => x.BranchId == branchId)
            .ToDictionaryAsync(x => x.ItemCode);

        // Read the Excel file using MiniExcel
        var rows = stream.Query<ItemImportDto>();

        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.ItemCode)) continue;

            // Normalize CoilRelationship
            var relationship = row.CoilRelationship;
            if (string.IsNullOrWhiteSpace(relationship)) relationship = "None";

            // Derive UOM
            // Logic: "Derive UOM: PCS -> SHEET, LBS -> COIL"
            // Re-interpreting: If the Item is a Coil (e.g. relationship is Parent/Child), UOM is LBS.
            // If it's a Sheet (relationship None or Description contains Sheet?), UOM is PCS.
            // The prompt "ItemMaster Import: Fields: ItemCode, Description, CoilRelationship. Derive UOM: PCS -> SHEET, LBS -> COIL"
            // is likely saying: The user uploads columns for Code, Desc, Rel.
            // But the *system* needs to derive UOM.
            // If the item is a COIL (based on relationship or description), UOM is LBS.
            // If the item is a SHEET (everything else?), UOM is PCS.

            string uom;
            if (relationship.Equals("Parent", StringComparison.OrdinalIgnoreCase) ||
                relationship.Equals("Child", StringComparison.OrdinalIgnoreCase) ||
                row.Description.Contains("COIL", StringComparison.OrdinalIgnoreCase))
            {
                uom = "LBS";
            }
            else
            {
                uom = "PCS";
            }

            if (existingItems.TryGetValue(row.ItemCode, out var item))
            {
                // Update existing
                item.Description = row.Description;
                item.CoilRelationship = relationship;
                item.Uom = uom;
            }
            else
            {
                // Create new
                var newItem = new ItemMaster
                {
                    BranchId = branchId,
                    ItemCode = row.ItemCode,
                    Description = row.Description,
                    CoilRelationship = relationship,
                    Uom = uom,
                    TotalWeightLbs = 0,
                    TotalQuantity = 0
                };
                db.ItemMasters.Add(newItem);
            }
        }

        await db.SaveChangesAsync();
    }

    private class ItemImportDto
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public string CoilRelationship { get; set; }
    }
}
