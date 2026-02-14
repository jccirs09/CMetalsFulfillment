using CMetalsFulfillment.Data.Entities;

namespace CMetalsFulfillment.Services.Configuration;

public interface IShippingConfigService
{
    // Trucks
    Task<List<Truck>> GetTrucksAsync(int branchId);
    Task CreateTruckAsync(Truck truck);
    Task UpdateTruckAsync(Truck truck);
    Task DeleteTruckAsync(int id, int branchId);

    // Regions/Groups/Rules
    Task<List<ShippingRegion>> GetRegionsAsync(int branchId);
    Task<List<ShippingGroup>> GetGroupsAsync(int branchId);
    Task<List<ShippingFsaRule>> GetFsaRulesAsync(int branchId);

    Task CreateRegionAsync(ShippingRegion region);
    Task CreateGroupAsync(ShippingGroup group);
    Task CreateFsaRuleAsync(ShippingFsaRule rule);

    Task<(ShippingRegion? Region, ShippingGroup? Group)> GetShippingDetailsAsync(string fsa, int branchId);
    string GetShippingMethod(string fob);
}
