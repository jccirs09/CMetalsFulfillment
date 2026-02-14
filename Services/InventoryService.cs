using CMetalsFulfillment.Data;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;

namespace CMetalsFulfillment.Services;

public class InventoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public InventoryService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task CreateSnapshotAsync(Stream stream, int branchId, string userId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        // Start transaction
        using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var header = new InventorySnapshotHeader
            {
                BranchId = branchId,
                UploadedByUserId = userId,
                UploadDate = DateTime.UtcNow
            };
            db.InventorySnapshotHeaders.Add(header);
            await db.SaveChangesAsync();

            var rows = stream.Query<InventorySnapshotDto>().ToList(); // Materialize to avoid streaming issues
            var aggregatedItems = new Dictionary<string, (decimal Qty, decimal Weight)>();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.ItemCode)) continue;

                string uom = "PCS";
                if (!string.IsNullOrWhiteSpace(row.Uom) && row.Uom.Equals("LBS", StringComparison.OrdinalIgnoreCase))
                {
                    uom = "LBS";
                }

                var snapshot = new InventorySnapshot
                {
                    BranchId = branchId,
                    SnapshotHeaderId = header.Id,
                    ItemCode = row.ItemCode,
                    Quantity = row.Quantity,
                    WeightLbs = row.WeightLbs,
                    Uom = uom,
                    SnapshotDate = DateTime.UtcNow
                };
                db.InventorySnapshots.Add(snapshot);

                if (!aggregatedItems.ContainsKey(row.ItemCode))
                {
                    aggregatedItems[row.ItemCode] = (0, 0);
                }
                var (currentQty, currentWeight) = aggregatedItems[row.ItemCode];
                aggregatedItems[row.ItemCode] = (currentQty + row.Quantity, currentWeight + row.WeightLbs);
            }

            await db.SaveChangesAsync();

            // Fetch existing items to update
            var itemCodes = aggregatedItems.Keys.ToList();
            var existingItems = await db.ItemMasters
                .Where(x => x.BranchId == branchId && itemCodes.Contains(x.ItemCode))
                .ToDictionaryAsync(x => x.ItemCode);

            foreach (var kvp in aggregatedItems)
            {
                if (existingItems.TryGetValue(kvp.Key, out var item))
                {
                    item.TotalQuantity = kvp.Value.Qty;
                    item.TotalWeightLbs = kvp.Value.Weight;
                }
                else
                {
                    // Create minimal item for missing master record
                    var newItem = new ItemMaster
                    {
                        BranchId = branchId,
                        ItemCode = kvp.Key,
                        Description = "Auto-created from Snapshot",
                        CoilRelationship = "None",
                        Uom = "PCS", // Default fallback
                        TotalQuantity = kvp.Value.Qty,
                        TotalWeightLbs = kvp.Value.Weight
                    };
                    db.ItemMasters.Add(newItem);
                }
            }

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<ItemMaster>> GetItemMastersAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ItemMasters.Where(x => x.BranchId == branchId).ToListAsync();
    }

    public async Task<List<InventorySnapshotHeader>> GetSnapshotsAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.InventorySnapshotHeaders
            .Include(x => x.UploadedByUser)
            .Where(x => x.BranchId == branchId)
            .OrderByDescending(x => x.UploadDate)
            .ToListAsync();
    }

    private class InventorySnapshotDto
    {
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal WeightLbs { get; set; }
        public string Uom { get; set; }
    }
}
