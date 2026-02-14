using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using CMetalsFulfillment.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class ShippingConfigurationService : IShippingConfigurationService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbFactory;

    public ShippingConfigurationService(IDbContextFactory<ApplicationDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    // Regions
    public async Task<List<ShippingRegion>> GetRegionsAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ShippingRegions.Where(r => r.BranchId == branchId).ToListAsync();
    }

    public async Task<ShippingRegion> CreateRegionAsync(ShippingRegion region)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        if (await db.ShippingRegions.AnyAsync(r => r.BranchId == region.BranchId && r.RegionCode == region.RegionCode))
            throw new InvalidOperationException("Region Code exists.");

        db.ShippingRegions.Add(region);
        await db.SaveChangesAsync();
        return region;
    }

    // Groups
    public async Task<List<ShippingGroup>> GetGroupsAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ShippingGroups.Where(g => g.BranchId == branchId).ToListAsync();
    }

    public async Task<ShippingGroup> CreateGroupAsync(ShippingGroup group)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        if (await db.ShippingGroups.AnyAsync(g => g.BranchId == group.BranchId && g.GroupCode == group.GroupCode))
            throw new InvalidOperationException("Group Code exists.");

        db.ShippingGroups.Add(group);
        await db.SaveChangesAsync();
        return group;
    }

    // FSA Rules
    public async Task<List<ShippingFsaRule>> GetFsaRulesAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ShippingFsaRules
            .Include(r => r.Region)
            .Include(r => r.Group)
            .Where(r => r.BranchId == branchId)
            .OrderBy(r => r.Priority)
            .ToListAsync();
    }

    public async Task<ShippingFsaRule> CreateFsaRuleAsync(ShippingFsaRule rule)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        // Normalize
        rule.FsaPrefix = rule.FsaPrefix.Trim().ToUpper();
        if (rule.FsaPrefix.Length > 3) rule.FsaPrefix = rule.FsaPrefix.Substring(0, 3);

        if (await db.ShippingFsaRules.AnyAsync(r => r.BranchId == rule.BranchId && r.FsaPrefix == rule.FsaPrefix && r.Priority == rule.Priority))
            throw new InvalidOperationException("Duplicate rule for this priority.");

        db.ShippingFsaRules.Add(rule);
        await db.SaveChangesAsync();
        return rule;
    }

    public async Task<ShippingFsaRule?> FindMatchingFsaRuleAsync(int branchId, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(postalCode)) return null;

        // Normalize: uppercase, remove spaces, take first 3
        var normalized = postalCode.ToUpper().Replace(" ", "");
        if (normalized.Length < 3) return null;
        var prefix = normalized.Substring(0, 3);

        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ShippingFsaRules
            .Include(r => r.Region)
            .Include(r => r.Group)
            .Where(r => r.BranchId == branchId && r.FsaPrefix == prefix && r.IsActive)
            .OrderBy(r => r.Priority)
            .FirstOrDefaultAsync();
    }

    // FOB Mappings
    public async Task<List<ShippingFobMapping>> GetFobMappingsAsync(int branchId)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ShippingFobMappings.Where(m => m.BranchId == branchId).ToListAsync();
    }

    public async Task<ShippingFobMapping> CreateFobMappingAsync(ShippingFobMapping mapping)
    {
        using var db = await _dbFactory.CreateDbContextAsync();

        mapping.FobToken = mapping.FobToken.Trim().ToUpper();

        if (await db.ShippingFobMappings.AnyAsync(m => m.BranchId == mapping.BranchId && m.FobToken == mapping.FobToken))
            throw new InvalidOperationException("FOB Token mapping exists.");

        db.ShippingFobMappings.Add(mapping);
        await db.SaveChangesAsync();
        return mapping;
    }

    public async Task<ShippingMethod?> GetShippingMethodForFobAsync(int branchId, string fobToken)
    {
        if (string.IsNullOrWhiteSpace(fobToken)) return null;

        var token = fobToken.Trim().ToUpper();

        using var db = await _dbFactory.CreateDbContextAsync();
        var mapping = await db.ShippingFobMappings
            .Where(m => m.BranchId == branchId && m.FobToken == token && m.IsActive)
            .FirstOrDefaultAsync();

        return mapping?.ShippingMethod;
    }
}
