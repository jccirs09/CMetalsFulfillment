namespace CMetalsFulfillment.Services.Inventory;

public interface IItemMasterService
{
    Task ImportItemMasterAsync(Stream excelStream, int branchId);
}
