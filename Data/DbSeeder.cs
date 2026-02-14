using CMetalsFulfillment.Domain.Constants;
using CMetalsFulfillment.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Ensure Database Created (Apply Migrations)
        await context.Database.MigrateAsync();

        // 1. Seed Branch
        var hqBranch = await context.Branches.FirstOrDefaultAsync(b => b.Name == "Head Office");
        if (hqBranch == null)
        {
            hqBranch = new Branch { Name = "Head Office", IsActive = true };
            context.Branches.Add(hqBranch);
            await context.SaveChangesAsync();
        }

        // 2. Seed SystemAdmin User
        var adminEmail = "admin@metalflow.local";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                DefaultBranchId = hqBranch.Id
            };
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to seed admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // 3. Seed Membership & Role for SystemAdmin
        // Check membership
        var membership = await context.UserBranchMemberships
            .FirstOrDefaultAsync(m => m.UserId == adminUser.Id && m.BranchId == hqBranch.Id);

        if (membership == null)
        {
            membership = new UserBranchMembership
            {
                UserId = adminUser.Id,
                BranchId = hqBranch.Id,
                IsActive = true,
                DefaultForUser = true
            };
            context.UserBranchMemberships.Add(membership);
        }
        else
        {
             if (!membership.DefaultForUser)
             {
                 membership.DefaultForUser = true;
             }
        }

        // Check Role
        var role = await context.UserBranchRoles
            .FirstOrDefaultAsync(r => r.UserId == adminUser.Id && r.BranchId == hqBranch.Id && r.RoleName == BranchRole.SystemAdmin);

        if (role == null)
        {
            role = new UserBranchRole
            {
                UserId = adminUser.Id,
                BranchId = hqBranch.Id,
                RoleName = BranchRole.SystemAdmin,
                IsActive = true
            };
            context.UserBranchRoles.Add(role);
        }

        await context.SaveChangesAsync();
    }
}
