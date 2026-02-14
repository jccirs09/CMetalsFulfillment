using CMetalsFulfillment.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CMetalsFulfillment.Data
{
    public static class Seeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            var userBranchService = scope.ServiceProvider.GetRequiredService<IUserBranchService>();

            // 1. Seed Roles
            string[] roles = { "SystemAdmin", "BranchAdmin", "Supervisor", "Planner", "Operator", "LoaderChecker", "Driver", "Viewer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Seed Branches
            using var context = await contextFactory.CreateDbContextAsync();
            var branches = new List<Branch>
            {
                new() { Code = "DLT", Name = "Delta", City = "Delta", Province = "BC", Address1 = "7630 Berg Road", PostalCode = "V4G 1G4", Phone = "604-946-3890", TimeZoneId = "America/Vancouver" },
                new() { Code = "SRY", Name = "Surrey", City = "Surrey", Province = "BC", Address1 = "#104 – 19433 96th Avenue", PostalCode = "V4N 4C4", Phone = "604-881-4500", TimeZoneId = "America/Vancouver" },
                new() { Code = "CGY", Name = "Calgary", City = "Calgary", Province = "AB", Address1 = "5566 54 Ave SE", PostalCode = "T2C 3A5", Phone = "403-279-4995", TimeZoneId = "America/Edmonton" },
                new() { Code = "YEG", Name = "Edmonton", City = "Edmonton", Province = "AB", Address1 = "9525 60 Avenue NW", PostalCode = "T6E 0C3", Phone = "780-962-9006", TimeZoneId = "America/Edmonton" },
                new() { Code = "YXE", Name = "Saskatoon", City = "Saskatoon", Province = "SK", Address1 = "3062 Millar Ave", PostalCode = "S7K 5X9", Phone = "306-652-2210", TimeZoneId = "America/Regina" },
                new() { Code = "YBR", Name = "Brandon", City = "Brandon", Province = "MB", Address1 = "33rd St East & Hwy 10", PostalCode = "R7A 5Y4", Phone = "204-728-9484", TimeZoneId = "America/Winnipeg" },
                new() { Code = "YWG", Name = "Winnipeg", City = "Winnipeg", Province = "MB", Address1 = "1540 Seel Ave", PostalCode = "R3T 4Z6", Phone = "204-477-8748", TimeZoneId = "America/Winnipeg" },
                new() { Code = "YHM", Name = "Hamilton", City = "Hamilton", Province = "ON", Address1 = "1632 Burlington St E", PostalCode = "L8H 3L3", Phone = "905-795-1880", TimeZoneId = "America/Toronto" },
                new() { Code = "YUL", Name = "Dorval", City = "Dorval", Province = "QC", Address1 = "1535 Hymus Blvd", PostalCode = "H9P 1J5", Phone = "514-532-0290", TimeZoneId = "America/Toronto" }
            };

            foreach (var b in branches)
            {
                var existing = await context.Branches.FirstOrDefaultAsync(x => x.Code == b.Code);
                if (existing == null)
                {
                    b.CreatedAtUtc = DateTime.UtcNow;
                    b.IsActive = true;
                    context.Branches.Add(b);
                }
                else
                {
                    // Update address/phone
                    existing.Address1 = b.Address1;
                    existing.Phone = b.Phone;
                    existing.PostalCode = b.PostalCode;
                    existing.City = b.City;
                    existing.Province = b.Province;
                    existing.Name = b.Name;
                    existing.TimeZoneId = b.TimeZoneId;
                }
            }
            await context.SaveChangesAsync();

            // 3. Seed SystemAdmin
            var email = configuration["Seed:SystemAdmin:Email"];
            var username = configuration["Seed:SystemAdmin:UserName"];
            var fullName = configuration["Seed:SystemAdmin:FullName"];
            var password = configuration["Seed:SystemAdmin:Password"];

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(username))
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = username,
                        Email = email,
                        FullName = fullName ?? "System Admin",
                        CreatedAtUtc = DateTime.UtcNow,
                        IsActive = true,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(user, password!);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to seed SystemAdmin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    if (!user.IsActive)
                    {
                        user.IsActive = true;
                        await userManager.UpdateAsync(user);
                    }
                }

                if (!await userManager.IsInRoleAsync(user, "SystemAdmin"))
                {
                    await userManager.AddToRoleAsync(user, "SystemAdmin");
                }

                // 4. Seed Memberships
                var defaultBranchCode = configuration["Seed:SystemAdmin:DefaultBranchCode"];
                var membershipBranchCodes = configuration["Seed:SystemAdmin:MembershipBranchCodes"]?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

                if (!string.IsNullOrEmpty(defaultBranchCode))
                {
                    var branch = await context.Branches.FirstOrDefaultAsync(b => b.Code == defaultBranchCode);
                    if (branch != null)
                    {
                        await userBranchService.AddMembershipAsync(user.Id, branch.Id, isDefault: true);
                    }
                }

                foreach (var code in membershipBranchCodes)
                {
                    if (code.Trim() == defaultBranchCode) continue;

                    var branch = await context.Branches.FirstOrDefaultAsync(b => b.Code == code.Trim());
                    if (branch != null)
                    {
                        await userBranchService.AddMembershipAsync(user.Id, branch.Id, isDefault: false);
                    }
                }
            }
        }
    }
}
