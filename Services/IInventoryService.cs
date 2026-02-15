using CMetalsFulfillment.Data.Entities;
using CMetalsFulfillment.Data.Enums;

namespace CMetalsFulfillment.Services;

public interface IInventoryService
{
    // Item Master
    Task<List<ItemMaster>> GetItemsAsync(int branchId, string search = "");
    Task<ItemMaster?> GetItemBySkuAsync(int branchId, string sku);
    Task ImportItemsAsync(int branchId, Stream fileStream);

    // Tags
    Task<List<Tag>> GetTagsAsync(int branchId, string search = "", TagStatus? status = null);
    Task<Tag?> GetTagByNumberAsync(int branchId, string tagNumber);
    Task<Tag> ReceiveTagAsync(int branchId, string sku, string tagNumber, decimal weightNet, decimal weightGross, string? location, string? notes, string userId);
    Task<Tag> MoveTagAsync(int branchId, string tagNumber, string newLocation, string userId);
    Task<Tag> AdjustTagStatusAsync(int branchId, string tagNumber, TagStatus newStatus, string userId, string reason);
}
