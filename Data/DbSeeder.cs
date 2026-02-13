using Microsoft.AspNetCore.Identity;
using CMetalsFulfillment.Domain;
using System.Security.Claims;
using CMetalsFulfillment.Features.Auth;

namespace CMetalsFulfillment.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var db = services.GetRequiredService<ApplicationDbContext>();

            // Seed Branch
            if (!db.Branches.Any())
            {
                db.Branches.Add(new Branch { Name = "Main Branch", IsActive = true });
                await db.SaveChangesAsync();

                // Also BranchSettings
                var branch = db.Branches.First();
                db.BranchSettings.Add(new BranchSettings { BranchId = branch.BranchId });
                await db.SaveChangesAsync();
            }

            // Seed SystemAdmin
            var adminEmail = "admin@metalflow.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (!result.Succeeded) throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                // Add Global SystemAdmin Claim
                await userManager.AddClaimAsync(adminUser, new Claim(ClaimTypes.Role, AuthConstants.RoleSystemAdmin));

                // Add to Branch
                var branch = db.Branches.First();
                db.UserBranchMemberships.Add(new UserBranchMembership
                {
                    UserId = adminUser.Id,
                    BranchId = branch.BranchId,
                    IsActive = true,
                    DefaultForUser = true
                });

                // Add BranchAdmin role
                db.UserBranchClaims.Add(new UserBranchClaim
                {
                    UserId = adminUser.Id,
                    BranchId = branch.BranchId,
                    ClaimType = "role",
                    ClaimValue = AuthConstants.RoleBranchAdmin,
                    IsActive = true
                });

                await db.SaveChangesAsync();
            }
        }
    }
}
