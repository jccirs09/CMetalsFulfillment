using CMetalsFulfillment.Data.Entities;
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
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await context.Database.MigrateAsync();

        // Seed Branches
        if (!await context.Branches.AnyAsync())
        {
            var branches = new List<Branch>
            {
                new() { Code = "DLT", Name = "Delta", Province = "BC", City = "Delta", Address1 = "7630 Berg Road", PostalCode = "V4G 1G4", Phone = "604-946-3890", TimeZoneId = "America/Vancouver" },
                new() { Code = "SRY", Name = "Surrey", Province = "BC", City = "Surrey", Address1 = "#104 – 19433 96th Avenue", PostalCode = "V4N 4C4", Phone = "604-881-4500", TimeZoneId = "America/Vancouver" },
                new() { Code = "CGY", Name = "Calgary", Province = "AB", City = "Calgary", Address1 = "5566 54 Ave SE", PostalCode = "T2C 3A5", Phone = "403-279-4995", TimeZoneId = "America/Edmonton" },
                new() { Code = "YEG", Name = "Edmonton", Province = "AB", City = "Edmonton", Address1 = "9525 60 Avenue NW", PostalCode = "T6E 0C3", Phone = "780-962-9006", TimeZoneId = "America/Edmonton" },
                new() { Code = "YXE", Name = "Saskatoon", Province = "SK", City = "Saskatoon", Address1 = "3062 Millar Ave", PostalCode = "S7K 5X9", Phone = "306-652-2210", TimeZoneId = "America/Regina" },
                new() { Code = "YBR", Name = "Brandon", Province = "MB", City = "Brandon", Address1 = "33rd St East & Hwy 10", PostalCode = "R7A 5Y4", Phone = "204-728-9484", TimeZoneId = "America/Winnipeg" },
                new() { Code = "YWG", Name = "Winnipeg", Province = "MB", City = "Winnipeg", Address1 = "1540 Seel Ave", PostalCode = "R3T 4Z6", Phone = "204-477-8748", TimeZoneId = "America/Winnipeg" },
                new() { Code = "YHM", Name = "Hamilton", Province = "ON", City = "Hamilton", Address1 = "1632 Burlington St E", PostalCode = "L8H 3L3", Phone = "905-795-1880", TimeZoneId = "America/Toronto" },
                new() { Code = "YUL", Name = "Dorval", Province = "QC", City = "Dorval", Address1 = "1535 Hymus Blvd", PostalCode = "H9P 1J5", Phone = "514-532-0290", TimeZoneId = "America/Toronto" }
            };
            context.Branches.AddRange(branches);
            await context.SaveChangesAsync();
        }

        // Seed Roles
        if (!await roleManager.RoleExistsAsync("SystemAdmin"))
        {
            await roleManager.CreateAsync(new IdentityRole("SystemAdmin"));
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
                FullName = "System Administrator",
                IsActive = true,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "SystemAdmin");

                // Add Membership
                var dltBranch = await context.Branches.FirstOrDefaultAsync(b => b.Code == "SRY");
                if (dltBranch != null)
                {
                    context.UserBranchMemberships.Add(new UserBranchMembership
                    {
                        UserId = adminUser.Id,
                        BranchId = dltBranch.Id,
                        IsDefaultForUser = true,
                        IsActive = true
                    });

                    context.UserBranchRoles.Add(new UserBranchRole
                    {
                        UserId = adminUser.Id,
                        BranchId = dltBranch.Id,
                        RoleName = "BranchAdmin",
                        AssignedByUserId = adminUser.Id
                    });

                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
