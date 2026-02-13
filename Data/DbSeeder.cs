using CMetalsFulfillment.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed Roles
            string[] roles = { "SystemAdmin", "BranchAdmin", "Planner", "Supervisor", "Operator", "LoaderChecker", "Driver" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Default Branch
            var hqBranch = await context.Branches.FirstOrDefaultAsync(b => b.Name == "Headquarters");
            if (hqBranch == null)
            {
                hqBranch = new Branch { Name = "Headquarters", IsActive = true };
                context.Branches.Add(hqBranch);
                await context.SaveChangesAsync();

                // Seed BranchSettings
                context.BranchSettings.Add(new BranchSettings { BranchId = hqBranch.BranchId });
                await context.SaveChangesAsync();
            }

            // Seed SystemAdmin User
            var adminEmail = "admin@metalflow.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DefaultBranchId = hqBranch.BranchId
                };
                var result = await userManager.CreateAsync(adminUser, "MetalFlow2024!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "SystemAdmin");

                    // Assign to HQ as BranchAdmin
                    context.UserBranchMemberships.Add(new UserBranchMembership
                    {
                        UserId = adminUser.Id,
                        BranchId = hqBranch.BranchId,
                        IsActive = true,
                        DefaultForUser = true
                    });

                    context.UserBranchClaims.Add(new UserBranchClaim
                    {
                        UserId = adminUser.Id,
                        BranchId = hqBranch.BranchId,
                        ClaimType = "role",
                        ClaimValue = "BranchAdmin",
                        IsActive = true
                    });

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
