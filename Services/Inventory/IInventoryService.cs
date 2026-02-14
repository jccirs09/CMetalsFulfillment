namespace CMetalsFulfillment.Services.Inventory;

public interface IInventoryService
{
    Task ImportSnapshotAsync(Stream excelStream, string filename, int branchId, string userId);
}
