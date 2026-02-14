using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services;

public class DataSeeder(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    ILogger<DataSeeder> logger)
{
    public async Task SeedAsync()
    {
        // 1. Seed Branches
        if (!await context.Branches.AnyAsync())
        {
            logger.LogInformation("Seeding Branches...");
            var branches = new List<Branch>
            {
                new() { Name = "Delta", TimezoneId = "America/Vancouver" },
                new() { Name = "Surrey", TimezoneId = "America/Vancouver" },
                new() { Name = "Calgary", TimezoneId = "America/Edmonton" },
                new() { Name = "Edmonton", TimezoneId = "America/Edmonton" },
                new() { Name = "Saskatoon", TimezoneId = "America/Regina" },
                new() { Name = "Brandon", TimezoneId = "America/Winnipeg" },
                new() { Name = "Winnipeg", TimezoneId = "America/Winnipeg" },
                new() { Name = "Hamilton", TimezoneId = "America/Toronto" },
                new() { Name = "Dorval", TimezoneId = "America/Toronto" }
            };

            context.Branches.AddRange(branches);
            await context.SaveChangesAsync();
            logger.LogInformation("Seeded {Count} Branches.", branches.Count);
        }

        // 2. Seed SystemAdmin Role
        if (!await roleManager.RoleExistsAsync("SystemAdmin"))
        {
            logger.LogInformation("Creating SystemAdmin Role...");
            await roleManager.CreateAsync(new IdentityRole("SystemAdmin"));
        }

        // 3. Seed Admin User
        var adminEmail = "admin@cmetals.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        // Find Delta branch (or first available if not found)
        var deltaBranch = await context.Branches.FirstOrDefaultAsync(b => b.Name == "Delta");

        if (adminUser == null)
        {
            logger.LogInformation("Seeding Admin User...");

            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DefaultBranchId = deltaBranch?.Id
            };

            var result = await userManager.CreateAsync(adminUser, "Password123!");
            if (result.Succeeded)
            {
                // Assign Global Role
                await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
                logger.LogInformation("Admin User Created: {Email}", adminEmail);

                // Assign Branch Membership & Role
                if (deltaBranch != null)
                {
                    var membership = new UserBranchMembership
                    {
                        UserId = adminUser.Id,
                        BranchId = deltaBranch.Id,
                        DefaultForUser = true
                    };
                    context.UserBranchMemberships.Add(membership);
                    await context.SaveChangesAsync();

                    context.UserBranchRoles.Add(new UserBranchRole
                    {
                        UserBranchMembershipId = membership.Id,
                        RoleName = "BranchAdmin"
                    });
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                 logger.LogError("Failed to create Admin User: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            // If user exists, ensure membership/role for Delta? (Optional, skipping to avoid complexity in seed)
        }

        // 4. Seed Phase 2 Configuration for Delta Branch
        if (deltaBranch != null)
        {
            // Shifts
            if (!await context.ShiftTemplates.AnyAsync(s => s.BranchId == deltaBranch.Id))
            {
                logger.LogInformation("Seeding Shift Templates for Delta...");
                context.ShiftTemplates.AddRange(
                    new ShiftTemplate
                    {
                        BranchId = deltaBranch.Id,
                        Name = "Day Shift",
                        StartTime = new TimeSpan(7, 0, 0),
                        EndTime = new TimeSpan(15, 30, 0),
                        IsOvernight = false
                    },
                    new ShiftTemplate
                    {
                        BranchId = deltaBranch.Id,
                        Name = "Night Shift",
                        StartTime = new TimeSpan(23, 0, 0),
                        EndTime = new TimeSpan(7, 0, 0),
                        IsOvernight = true
                    }
                );
                await context.SaveChangesAsync();
            }

            // Machines
            if (!await context.Machines.AnyAsync(m => m.BranchId == deltaBranch.Id))
            {
                logger.LogInformation("Seeding Machines for Delta...");
                context.Machines.Add(new Machine
                {
                    BranchId = deltaBranch.Id,
                    Name = "Saw 1",
                    Description = "Primary Band Saw",
                    Status = "Active"
                });
                await context.SaveChangesAsync();
            }

            // Stations
            if (!await context.PickPackStations.AnyAsync(s => s.BranchId == deltaBranch.Id))
            {
                logger.LogInformation("Seeding Pick/Pack Stations for Delta...");
                context.PickPackStations.Add(new PickPackStation
                {
                    BranchId = deltaBranch.Id,
                    Name = "Pack Station 1",
                    Status = "Active"
                });
                await context.SaveChangesAsync();
            }

            // Trucks
            if (!await context.Trucks.AnyAsync(t => t.BranchId == deltaBranch.Id))
            {
                logger.LogInformation("Seeding Trucks for Delta...");
                context.Trucks.Add(new Truck
                {
                    BranchId = deltaBranch.Id,
                    Name = "Truck 101",
                    LicensePlate = "BC-123-XYZ",
                    MaxWeightLbs = 10000,
                    Status = "Active"
                });
                await context.SaveChangesAsync();
            }

            // Shipping Regions/Groups/Rules
            if (!await context.ShippingRegions.AnyAsync(r => r.BranchId == deltaBranch.Id))
            {
                logger.LogInformation("Seeding Shipping Config for Delta...");

                var region = new ShippingRegion { BranchId = deltaBranch.Id, Name = "Local", Description = "Greater Vancouver" };
                var group = new ShippingGroup { BranchId = deltaBranch.Id, Name = "Zone 1", Description = "Delta/Surrey" };

                context.ShippingRegions.Add(region);
                context.ShippingGroups.Add(group);
                await context.SaveChangesAsync(); // Commit to get IDs

                context.ShippingFsaRules.Add(new ShippingFsaRule
                {
                    BranchId = deltaBranch.Id,
                    FsaPrefix = "V4G",
                    ShippingRegionId = region.Id,
                    ShippingGroupId = group.Id
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
