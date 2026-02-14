using CMetalsFulfillment.Data;
using CMetalsFulfillment.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Services
{
    public class SeedService(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<SeedService> logger)
    {
        public async Task InitializeAsync()
        {
            await EnsureRolesAsync();
            await EnsureBranchesAsync();
            await EnsureSystemAdminAsync();
        }

        private async Task EnsureRolesAsync()
        {
            var roles = new[] { "SystemAdmin", "BranchAdmin", "Supervisor", "Planner", "Operator", "LoaderChecker", "Driver", "Viewer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Created role: {Role}", role);
                }
            }
        }

        private async Task EnsureBranchesAsync()
        {
            using var context = await dbContextFactory.CreateDbContextAsync();

            // Canada Branches
            var branches = new List<Branch>
            {
                new() { Code = "DLT", Name = "Delta", Country = "Canada", ProvinceState = "BC", City = "Delta", Address1 = "7630 Berg Road", PostalCode = "V4G 1G4", Phone = "(604) 946-3890", TimeZoneId = "America/Vancouver" },
                new() { Code = "SRY", Name = "Surrey", Country = "Canada", ProvinceState = "BC", City = "Surrey", Address1 = "123 Surrey St", PostalCode = "V3V 3V3", Phone = "(604) 555-0101", TimeZoneId = "America/Vancouver" },
                new() { Code = "CGY", Name = "Calgary", Country = "Canada", ProvinceState = "AB", City = "Calgary", Address1 = "123 Calgary Ave", PostalCode = "T2P 2M5", Phone = "(403) 555-0102", TimeZoneId = "America/Edmonton" },
                new() { Code = "EDM", Name = "Edmonton", Country = "Canada", ProvinceState = "AB", City = "Edmonton", Address1 = "123 Edmonton Blvd", PostalCode = "T5J 0N3", Phone = "(780) 555-0103", TimeZoneId = "America/Edmonton" },
                new() { Code = "SAS", Name = "Saskatoon", Country = "Canada", ProvinceState = "SK", City = "Saskatoon", Address1 = "123 Saskatoon Dr", PostalCode = "S7K 3J6", Phone = "(306) 555-0104", TimeZoneId = "America/Regina" },
                new() { Code = "BRN", Name = "Brandon", Country = "Canada", ProvinceState = "MB", City = "Brandon", Address1 = "#410, Box 7, R.R. 4, 33rd Street East & Hwy 10", PostalCode = "R7A 5Y4", Phone = "204-728-9484", TimeZoneId = "America/Winnipeg" },
                new() { Code = "WPG", Name = "Winnipeg", Country = "Canada", ProvinceState = "MB", City = "Winnipeg", Address1 = "123 Winnipeg Way", PostalCode = "R3C 4T6", Phone = "(204) 555-0105", TimeZoneId = "America/Winnipeg" },
                new() { Code = "HAM", Name = "Hamilton", Country = "Canada", ProvinceState = "ON", City = "Hamilton", Address1 = "1632 Burlington St E", PostalCode = "L8H 3L3", Phone = "905-795-1880", TimeZoneId = "America/Toronto" },
                new() { Code = "MTL", Name = "Montreal", Country = "Canada", ProvinceState = "QC", City = "Dorval", Address1 = "1535 Hymus Blvd", PostalCode = "H9P 1J5", Phone = "(514) 555-0106", TimeZoneId = "America/Toronto" },
                new() { Code = "MIS", Name = "Mississauga", Country = "Canada", ProvinceState = "ON", City = "Mississauga", Address1 = "123 Mississauga Rd", PostalCode = "L5B 2C9", Phone = "(905) 555-0107", TimeZoneId = "America/Toronto" }
            };

            foreach (var b in branches)
            {
                if (!await context.Branches.AnyAsync(x => x.Code == b.Code))
                {
                    context.Branches.Add(b);
                    logger.LogInformation("Seeded branch: {Name} ({Code})", b.Name, b.Code);
                }
            }
            await context.SaveChangesAsync();
        }

        private async Task EnsureSystemAdminAsync()
        {
            var email = configuration["Seed:SystemAdmin:Email"];
            var userName = configuration["Seed:SystemAdmin:UserName"];
            var fullName = configuration["Seed:SystemAdmin:FullName"];
            var password = configuration["Seed:SystemAdmin:Password"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                logger.LogWarning("SystemAdmin seed data missing in configuration.");
                return;
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName ?? email,
                    Email = email,
                    FullName = fullName ?? "System Administrator",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    logger.LogInformation("Created SystemAdmin user: {Email}", email);
                }
                else
                {
                    logger.LogError("Failed to create SystemAdmin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(user, "SystemAdmin"))
            {
                await userManager.AddToRoleAsync(user, "SystemAdmin");
                logger.LogInformation("Assigned SystemAdmin role to user: {Email}", email);
            }
        }
    }
}
