using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;

namespace CMetalsFulfillment.Services.Inventory;

public class InventoryService(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IInventoryService
{
    public async Task ImportSnapshotAsync(Stream excelStream, string filename, int branchId, string userId)
    {
        using var context = await dbContextFactory.CreateDbContextAsync();

        // 1. Create Snapshot Record
        var snapshot = new InventorySnapshot
        {
            BranchId = branchId,
            UploadedAtUtc = DateTime.UtcNow,
            UploadedByUserId = userId,
            Filename = filename
        };
        context.InventorySnapshots.Add(snapshot);
        await context.SaveChangesAsync(); // Commit to get Snapshot ID

        // 2. Read Excel Stream (By Index)
        var rows = excelStream.Query(useHeaderRow: false).ToList(); // Use raw index-based access

        var snapshotLines = new List<InventorySnapshotLine>();
        var stockLines = new List<InventoryStock>();

        // Skip header row (index 0)
        for (int i = 1; i < rows.Count; i++)
        {
            var row = (IDictionary<string, object>)rows[i];

            // Map by Column Index (0-based)
            // A=0, B=1, C=2, D=3, E=4, F=5, G=6, H=7, I=8, J=9, K=10, L=11, M=12, N=13
            // 1) Item ID (A) -> ItemCode
            var itemCode = GetValue(row, "A");
            // 2) Description (B) -> Description
            var description = GetValue(row, "B");
            // 3) Size (C) -> Size
            var size = GetValue(row, "C");
            // 5) Tag # (E) -> TagNumber
            var tagNumber = GetValue(row, "E");
            // 8) Snapshot Loc (H) -> Location
            var location = GetValue(row, "H");
            // 10) Snapshot Qty (J) -> Quantity
            var qtyStr = GetValue(row, "J");
            decimal.TryParse(qtyStr, out var quantity);
            // 13) [BLANK] -> UOM (M) -> Uom
            var uom = GetValue(row, "M")?.ToUpperInvariant();

            if (string.IsNullOrWhiteSpace(itemCode)) continue;

            // Validate UOM
            if (uom != "PCS" && uom != "LBS")
            {
                // Reject row per requirement.
                continue;
            }

            var line = new InventorySnapshotLine
            {
                BranchId = branchId,
                SnapshotId = snapshot.Id,
                ItemCode = itemCode!,
                Description = description ?? "",
                Size = size ?? "",
                TagNumber = tagNumber ?? "",
                Location = location ?? "",
                Quantity = quantity,
                Uom = uom!
            };
            snapshotLines.Add(line);

            stockLines.Add(new InventoryStock
            {
                BranchId = branchId,
                ItemCode = itemCode!,
                TagNumber = tagNumber ?? "",
                Location = location ?? "",
                Quantity = quantity,
                Uom = uom!,
                LastSnapshotId = snapshot.Id
            });
        }

        // Batch Insert Snapshot Lines
        context.InventorySnapshotLines.AddRange(snapshotLines);

        // Update InventoryStock (Full Replace for Branch)
        // 1. Delete Existing
        await context.InventoryStocks.Where(s => s.BranchId == branchId).ExecuteDeleteAsync();

        // 2. Insert New
        context.InventoryStocks.AddRange(stockLines);

        await context.SaveChangesAsync();
    }

    private string? GetValue(IDictionary<string, object> row, string key)
    {
        if (row.TryGetValue(key, out var value) && value != null)
        {
            return value.ToString()?.Trim();
        }
        return null;
    }
}
