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
        if (adminUser == null)
        {
            logger.LogInformation("Seeding Admin User...");

            // Get Delta branch for default
            var branch = await context.Branches.FirstOrDefaultAsync(b => b.Name == "Delta");

            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DefaultBranchId = branch?.Id
            };

            var result = await userManager.CreateAsync(adminUser, "Password123!");
            if (result.Succeeded)
            {
                // Assign Global Role
                await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
                logger.LogInformation("Admin User Created: {Email}", adminEmail);

                // Assign Branch Membership & Role
                if (branch != null)
                {
                    var membership = new UserBranchMembership
                    {
                        UserId = adminUser.Id,
                        BranchId = branch.Id,
                        DefaultForUser = true
                    };
                    context.UserBranchMemberships.Add(membership);
                    await context.SaveChangesAsync();

                    // Reload membership to get ID? No, EF Core populates it.
                    // But to be safe with context tracking...

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
    }
}
