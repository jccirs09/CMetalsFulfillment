using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using CMetalsFulfillment.Data.Enums;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;

namespace CMetalsFulfillment.Services;

public class InventoryService : IInventoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;
    private readonly IAuditService _auditService;

    public InventoryService(IDbContextFactory<ApplicationDbContext> dbFactory, IAuditService auditService)
    {
        _dbFactory = dbFactory;
        _auditService = auditService;
    }

    public async Task<List<ItemMaster>> GetItemsAsync(int branchId, string search = "")
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.ItemMasters.Where(i => i.BranchId == branchId && i.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(i => i.Sku.Contains(search) || i.Description.Contains(search));
        }

        return await query.OrderBy(i => i.Sku).ToListAsync();
    }

    public async Task<ItemMaster?> GetItemBySkuAsync(int branchId, string sku)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ItemMasters.FirstOrDefaultAsync(i => i.BranchId == branchId && i.Sku == sku);
    }

    public async Task ImportItemsAsync(int branchId, Stream fileStream)
    {
        // Use concrete type for MiniExcel query to avoid dynamic expression tree issues
        var rows = fileStream.Query(useHeaderRow: true).Cast<IDictionary<string, object>>().ToList();

        using var db = await _dbFactory.CreateDbContextAsync();

        foreach (var row in rows)
        {
            var sku = row.ContainsKey("Sku") ? row["Sku"]?.ToString() : null;
            var desc = row.ContainsKey("Description") ? row["Description"]?.ToString() : null;
            var uom = row.ContainsKey("Uom") ? row["Uom"]?.ToString() ?? "LB" : "LB";
            var weightStr = row.ContainsKey("WeightPerUnit") ? row["WeightPerUnit"]?.ToString() : null;

            if (string.IsNullOrWhiteSpace(sku) || string.IsNullOrWhiteSpace(desc)) continue;

            decimal.TryParse(weightStr, out decimal weight);

            var existing = await db.ItemMasters.FirstOrDefaultAsync(i => i.BranchId == branchId && i.Sku == sku);
            if (existing != null)
            {
                existing.Description = desc;
                existing.Uom = uom;
                existing.WeightPerUnit = weight;
                existing.UpdatedAtUtc = DateTime.UtcNow;
            }
            else
            {
                db.ItemMasters.Add(new ItemMaster
                {
                    BranchId = branchId,
                    Sku = sku,
                    Description = desc,
                    Uom = uom,
                    WeightPerUnit = weight,
                    IsActive = true
                });
            }
        }
        await db.SaveChangesAsync();
    }

    public async Task<List<Tag>> GetTagsAsync(int branchId, string search = "", TagStatus? status = null)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var query = db.Tags
            .Include(t => t.Item)
            .Where(t => t.BranchId == branchId);

        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t => t.TagNumber.Contains(search) || t.Item.Sku.Contains(search));
        }

        return await query.OrderByDescending(t => t.CreatedAtUtc).ToListAsync();
    }

    public async Task<Tag?> GetTagByNumberAsync(int branchId, string tagNumber)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Tags
            .Include(t => t.Item)
            .FirstOrDefaultAsync(t => t.BranchId == branchId && t.TagNumber == tagNumber);
    }

    public async Task<Tag> ReceiveTagAsync(int branchId, string sku, string tagNumber, decimal weightNet, decimal weightGross, string? location, string? notes, string userId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        var item = await db.ItemMasters.FirstOrDefaultAsync(i => i.BranchId == branchId && i.Sku == sku);
        if (item == null) throw new InvalidOperationException($"SKU '{sku}' not found.");

        if (await db.Tags.AnyAsync(t => t.BranchId == branchId && t.TagNumber == tagNumber))
            throw new InvalidOperationException($"Tag '{tagNumber}' already exists.");

        var tag = new Tag
        {
            BranchId = branchId,
            TagNumber = tagNumber,
            ItemMasterId = item.Id,
            Status = TagStatus.Received,
            WeightNet = weightNet,
            WeightGross = weightGross,
            Location = location,
            Notes = notes
        };

        db.Tags.Add(tag);
        await db.SaveChangesAsync();

        await _auditService.LogAsync(branchId, "TagReceived", "Tag", tagNumber, userId, null, $"{sku} - {weightNet}");
        return tag;
    }

    public async Task<Tag> MoveTagAsync(int branchId, string tagNumber, string newLocation, string userId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var tag = await db.Tags.FirstOrDefaultAsync(t => t.BranchId == branchId && t.TagNumber == tagNumber);

        if (tag == null) throw new KeyNotFoundException("Tag not found.");

        var oldLocation = tag.Location;
        tag.Location = newLocation;
        tag.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();

        await _auditService.LogAsync(branchId, "TagMoved", "Tag", tagNumber, userId, null, $"{oldLocation} -> {newLocation}");
        return tag;
    }

    public async Task<Tag> AdjustTagStatusAsync(int branchId, string tagNumber, TagStatus newStatus, string userId, string reason)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var tag = await db.Tags.FirstOrDefaultAsync(t => t.BranchId == branchId && t.TagNumber == tagNumber);

        if (tag == null) throw new KeyNotFoundException("Tag not found.");

        var oldStatus = tag.Status;
        tag.Status = newStatus;
        tag.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync();

        await _auditService.LogAsync(branchId, "TagStatusChanged", "Tag", tagNumber, userId, reason, $"{oldStatus} -> {newStatus}");
        return tag;
    }
}
