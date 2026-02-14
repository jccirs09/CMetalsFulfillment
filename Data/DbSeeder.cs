using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var dbFactory = services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        using var db = await dbFactory.CreateDbContextAsync();

        if (await db.Branches.AnyAsync())
        {
            return;
        }

        // Create Default Branch
        var mainBranch = new Branch { Name = "Main Branch", TimezoneId = "UTC" };
        db.Branches.Add(mainBranch);
        await db.SaveChangesAsync();

        // Create Admin User
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var adminEmail = "admin@metalflow.com";
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            DefaultBranchId = mainBranch.Id
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
             // Assign Membership and Role
             var membership = new UserBranchMembership
             {
                 UserId = adminUser.Id,
                 BranchId = mainBranch.Id,
                 IsDefault = true
             };
             db.BranchMemberships.Add(membership);
             await db.SaveChangesAsync();

             var role = new UserBranchRole
             {
                 MembershipId = membership.Id,
                 RoleName = "SystemAdmin"
             };
             db.BranchRoles.Add(role);
             await db.SaveChangesAsync();
        }
    }
}
