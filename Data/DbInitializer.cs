using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var contextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var context = await contextFactory.CreateDbContextAsync();

            await context.Database.MigrateAsync();

            // Seed Branches
            if (!await context.Branches.AnyAsync())
            {
                var branches = new[]
                {
                    new Branch { Code = "DLT", Name = "Delta", City = "Delta", Province = "BC", Address1 = "7630 Berg Road", PostalCode = "V4G 1G4", Phone = "604-946-3890", TimeZoneId = "America/Vancouver", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
                    new Branch { Code = "SRY", Name = "Surrey", City = "Surrey", Province = "BC", Address1 = "#104 – 19433 96th Avenue", PostalCode = "V4N 4C4", Phone = "604-881-4500", TimeZoneId = "America/Vancouver", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
                    new Branch { Code = "CGY", Name = "Calgary", City = "Calgary", Province = "AB", Address1 = "5566 54 Ave SE", PostalCode = "T2C 3A5", Phone = "403-279-4995", TimeZoneId = "America/Edmonton", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
                    new Branch { Code = "YEG", Name = "Edmonton", City = "Edmonton", Province = "AB", Address1 = "9525 60 Avenue NW", PostalCode = "T6E 0C3", Phone = "780-962-9006", TimeZoneId = "America/Edmonton", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
                    new Branch { Code = "YXE", Name = "Saskatoon", City = "Saskatoon", Province = "SK", Address1 = "3062 Millar Ave", PostalCode = "S7K 5X9", Phone = "306-652-2210", TimeZoneId = "America/Regina", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
                    new Branch { Code = "YBR", Name = "Brandon", City = "Brandon", Province = "MB", Address1 = "33rd St East & Hwy 10", PostalCode = "R7A 5Y4", Phone = "204-728-9484", TimeZoneId = "America/Winnipeg", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
                    new Branch { Code = "YWG", Name = "Winnipeg", City = "Winnipeg", Province = "MB", Address1 = "1540 Seel Ave", PostalCode = "R3T 4Z6", Phone = "204-477-8748", TimeZoneId = "America/Winnipeg", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
                    new Branch { Code = "YHM", Name = "Hamilton", City = "Hamilton", Province = "ON", Address1 = "1632 Burlington St E", PostalCode = "L8H 3L3", Phone = "905-795-1880", TimeZoneId = "America/Toronto", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow },
                    new Branch { Code = "YUL", Name = "Dorval", City = "Dorval", Province = "QC", Address1 = "1535 Hymus Blvd", PostalCode = "H9P 1J5", Phone = "514-532-0290", TimeZoneId = "America/Toronto", CreatedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow }
                };
                context.Branches.AddRange(branches);
                await context.SaveChangesAsync();
            }

            // Seed Roles
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = { "SystemAdmin", "BranchAdmin", "Supervisor", "Planner", "Operator", "LoaderChecker", "Driver", "Viewer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed SystemAdmin
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminEmail = "admin@metalflow.local"; // Default if config missing
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    IsActive = true
                };
                var result = await userManager.CreateAsync(adminUser, "Admin123!"); // Default password
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
                }
            }
        }
    }
}
