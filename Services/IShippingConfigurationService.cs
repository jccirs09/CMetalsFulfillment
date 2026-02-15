using CMetalsFulfillment.Data.Entities;
using CMetalsFulfillment.Data.Enums;

namespace CMetalsFulfillment.Services;

public interface IShippingConfigurationService
{
    // Regions
    Task<List<ShippingRegion>> GetRegionsAsync(int branchId);
    Task<ShippingRegion> CreateRegionAsync(ShippingRegion region);

    // Groups
    Task<List<ShippingGroup>> GetGroupsAsync(int branchId);
    Task<ShippingGroup> CreateGroupAsync(ShippingGroup group);

    // FSA Rules
    Task<List<ShippingFsaRule>> GetFsaRulesAsync(int branchId);
    Task<ShippingFsaRule> CreateFsaRuleAsync(ShippingFsaRule rule);
    Task<ShippingFsaRule?> FindMatchingFsaRuleAsync(int branchId, string postalCode);

    // FOB Mappings
    Task<List<ShippingFobMapping>> GetFobMappingsAsync(int branchId);
    Task<ShippingFobMapping> CreateFobMappingAsync(ShippingFobMapping mapping);
    Task<ShippingMethod?> GetShippingMethodForFobAsync(int branchId, string fobToken);
}
