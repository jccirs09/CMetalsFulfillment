using CMetalsFulfillment.Data.Entities;
using CMetalsFulfillment.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CMetalsFulfillment.Data;

public class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure Database Created
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Branch
        var defaultBranch = await context.Branches.FirstOrDefaultAsync(b => b.Code == "HQ");
        if (defaultBranch == null)
        {
            defaultBranch = new Branch
            {
                Name = "Headquarters",
                Code = "HQ",
                Settings = new BranchSettings { TimezoneId = "UTC" }
            };
            context.Branches.Add(defaultBranch);
            await context.SaveChangesAsync();
        }

        // 2. Seed SystemAdmin
        var adminEmail = "admin@metalflow.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DefaultBranchId = defaultBranch.BranchId
            };

            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // 3. Assign Membership & Role
        var membership = await context.BranchMemberships
            .FirstOrDefaultAsync(m => m.UserId == adminUser.Id && m.BranchId == defaultBranch.BranchId);

        if (membership == null)
        {
            membership = new UserBranchMembership
            {
                UserId = adminUser.Id,
                BranchId = defaultBranch.BranchId
            };
            context.BranchMemberships.Add(membership);
            await context.SaveChangesAsync();
        }

        var hasRole = await context.BranchRoles
            .AnyAsync(r => r.UserId == adminUser.Id && r.BranchId == defaultBranch.BranchId && r.RoleName == RoleConstants.SystemAdmin);

        if (!hasRole)
        {
            context.BranchRoles.Add(new UserBranchRole
            {
                UserId = adminUser.Id,
                BranchId = defaultBranch.BranchId,
                RoleName = RoleConstants.SystemAdmin,
                Membership = membership
            });
            await context.SaveChangesAsync();
        }
    }
}
