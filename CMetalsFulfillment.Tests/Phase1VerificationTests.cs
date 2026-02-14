using CMetalsFulfillment.Data;
using CMetalsFulfillment.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Moq;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;

namespace CMetalsFulfillment.Tests
{
    public class Phase1VerificationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _dbName;

        public Phase1VerificationTests(WebApplicationFactory<Program> factory)
        {
            _dbName = $"test_{Guid.NewGuid()}.db";

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");

                builder.UseDefaultServiceProvider(options =>
                {
                    options.ValidateScopes = false;
                    options.ValidateOnBuild = false;
                });

                builder.ConfigureServices(services =>
                {
                    // Use a separate test database
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite($"Data Source={_dbName}");
                    });

                    services.AddDbContextFactory<ApplicationDbContext>(options =>
                    {
                        options.UseSqlite($"Data Source={_dbName}");
                    });

                    services.AddScoped<IJSRuntime>(sp => new Mock<IJSRuntime>().Object);
                });
            });

            // Apply Migrations & Seed
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            // Seeder.SeedAsync is static
            Seeder.SeedAsync(scope.ServiceProvider, config).GetAwaiter().GetResult();
        }

        public class MockNavigationManager : NavigationManager
        {
            public MockNavigationManager(string uri)
            {
                Initialize(uri, uri);
            }
        }

        [Fact]
        public async Task A_B_StartupAndSeeding_VerifyDbContent()
        {
            // Act: Create scope to trigger startup and seeding
            using var scope = _factory.Services.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var context = await dbFactory.CreateDbContextAsync();

            // Assert 1: Migrations Applied (Tables exist)
            Assert.True(await context.Database.CanConnectAsync());

            // Assert 2: Identity Roles (SystemAdmin..Viewer)
            var roles = new[] { "SystemAdmin", "BranchAdmin", "Supervisor", "Planner", "Operator", "LoaderChecker", "Driver", "Viewer" };
            foreach (var role in roles)
            {
                Assert.True(await context.Roles.AnyAsync(r => r.Name == role), $"Role {role} missing");
            }

            // Assert 3: Cascadia Branches (9 count + specific codes)
            var expectedCodes = new[] { "DLT", "SRY", "CGY", "YEG", "YXE", "YBR", "YWG", "YHM", "YUL" };
            var branches = await context.Branches.ToListAsync();
            Assert.Equal(9, branches.Count);
            foreach (var code in expectedCodes)
            {
                var branch = branches.FirstOrDefault(b => b.Code == code);
                Assert.NotNull(branch);
                Assert.True(branch.IsActive);
                Assert.False(string.IsNullOrEmpty(branch.TimeZoneId));
                // Verify TimeZone mapping sample
                if (code == "DLT") Assert.Equal("America/Vancouver", branch.TimeZoneId);
                if (code == "YUL") Assert.Equal("America/Toronto", branch.TimeZoneId);
            }

            // Assert 4: SystemAdmin User
            var sysAdmin = await context.Users.FirstOrDefaultAsync(u => u.UserName == "sysadmin");
            Assert.NotNull(sysAdmin);
            Assert.True(sysAdmin.IsActive);
            Assert.Equal("System Administrator", sysAdmin.FullName);

            // Assert 5: SystemAdmin Memberships
            var memberships = await context.UserBranchMemberships.Where(m => m.UserId == sysAdmin.Id).ToListAsync();
            Assert.Contains(memberships, m => m.Branch.Code == "SRY" && m.IsDefaultForUser);
            Assert.Contains(memberships, m => m.Branch.Code == "DLT" && !m.IsDefaultForUser);
        }

        [Fact]
        public async Task C_E_AuthAndRoles_VerifyClaimsAndResolver()
        {
            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Actually registered as IClaimsTransformation
            var transform = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Authentication.IClaimsTransformation>();
            var roleResolver = scope.ServiceProvider.GetRequiredService<IRoleResolver>();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

            // Get SysAdmin
            using var context = await dbFactory.CreateDbContextAsync();
            var sysAdmin = await context.Users.FirstAsync(u => u.UserName == "sysadmin");

            // 1. Verify Claims Transformation
            var principal = await scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>().CreateAsync(sysAdmin);
            var transformed = await transform.TransformAsync(principal);

            Assert.Contains(transformed.Claims, c => c.Type == "mf:userId" && c.Value == sysAdmin.Id);
            Assert.Contains(transformed.Claims, c => c.Type == "mf:isSystemAdmin" && c.Value == "true");
            Assert.Contains(transformed.Claims, c => c.Type == "mf:defaultBranchId"); // ID depends on DB seed, but should exist

            // 2. Verify RoleResolver (SystemAdmin has no branch roles by default in seed, let's add one to test)
            var sryBranch = await context.Branches.FirstAsync(b => b.Code == "SRY");

            // Add a test role manually
            context.UserBranchRoles.Add(new UserBranchRole
            {
                UserId = sysAdmin.Id,
                BranchId = sryBranch.Id,
                RoleName = "Planner",
                AssignedByUserId = "seed",
                AssignedAtUtc = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            // Check
            Assert.True(await roleResolver.HasRoleAsync(sysAdmin.Id, sryBranch.Id, "Planner"));
            Assert.False(await roleResolver.HasRoleAsync(sysAdmin.Id, sryBranch.Id, "Operator"));
        }

        [Fact]
        public async Task D_BranchResolution_VerifyLogic()
        {
            // Unit testing BranchContext logic requires mocking NavManager, HttpContext, etc.
            // Since BranchContext is Scoped, we can resolve it but need to mock dependencies.

            var mockBranchService = new Mock<IBranchService>();
            var mockUserBranchService = new Mock<IUserBranchService>();
            var mockNav = new MockNavigationManager("http://localhost/");
            var mockHttp = new Mock<IHttpContextAccessor>();
            var mockJs = new Mock<IJSRuntime>();

            // Setup Data
            var userId = "user1";
            var branchId = 10;
            var branch = new Branch { Id = branchId, Code = "TST", TimeZoneId = "UTC", IsActive = true };

            mockBranchService.Setup(s => s.GetBranchByIdAsync(branchId)).ReturnsAsync(branch);
            mockUserBranchService.Setup(s => s.GetDefaultMembershipAsync(userId)).ReturnsAsync(new UserBranchMembership { BranchId = branchId });

            // Mock Cookie empty
            var context = new DefaultHttpContext();
            mockHttp.Setup(h => h.HttpContext).Returns(context);

            var branchContext = new BranchContext(
                mockBranchService.Object,
                mockUserBranchService.Object,
                mockNav,
                mockHttp.Object,
                mockJs.Object);

            await branchContext.ResolveAsync(userId);

            Assert.Equal(branchId, branchContext.ActiveBranchId);
            Assert.Equal("TST", branchContext.ActiveBranchCode);
        }

        [Fact]
        public async Task F_SetupGates_VerifyStatus()
        {
            using var scope = _factory.Services.CreateScope();
            var setupService = scope.ServiceProvider.GetRequiredService<ISetupStatusService>();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();

            using var context = await dbFactory.CreateDbContextAsync();
            var branch = await context.Branches.FirstAsync(b => b.Code == "SRY");

            // Check status (Should be incomplete)
            var complete = await setupService.IsSetupCompleteAsync(branch.Id);
            Assert.False(complete);

            var gates = await setupService.GetGateStatusAsync(branch.Id);
            Assert.True(gates["BranchActive"]);
            Assert.False(gates["BranchAdminExists"]); // Seed doesn't add BranchAdmin role to SRY for SysAdmin, only membership?
            // Wait, seed adds SystemAdmin role. Does it add BranchAdmin?
            // Seeder: "Always create a UserBranchMembership...". Does not mention adding BranchRole.

            // Add BranchAdmin role and verify gate
            // Create user first for FK
            var user = new ApplicationUser { UserName = "adminuser", Email = "admin@example.com", FullName = "Admin" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            context.UserBranchRoles.Add(new UserBranchRole
            {
                UserId = user.Id,
                BranchId = branch.Id,
                RoleName = "BranchAdmin",
                AssignedByUserId = "seed",
                AssignedAtUtc = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            gates = await setupService.GetGateStatusAsync(branch.Id);
            Assert.True(gates["BranchAdminExists"]);

            // Phase 2 gates should be false
            Assert.False(gates["MachinesConfigured"]);
        }

        [Fact]
        public async Task I_Concurrency_VerifySchema()
        {
            using var scope = _factory.Services.CreateScope();
            var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
            using var context = await dbFactory.CreateDbContextAsync();

            // Check metadata for Version property on ConcurrencyAware entities
            var entityType = context.Model.FindEntityType(typeof(PickingListHeader));
            var versionProp = entityType!.FindProperty("Version");
            Assert.NotNull(versionProp);
            Assert.True(versionProp.IsConcurrencyToken);

            // Test actual concurrency behavior
            var header = new PickingListHeader { Version = 1 };
            context.PickingListHeaders.Add(header);
            await context.SaveChangesAsync();

            var savedVersion = header.Version; // Should be 1 (or 0+1?) Logic says: if Modified -> ++. Added -> ?
            // In SaveChangesAsync override: "if (entry.State == EntityState.Modified)"
            // So Added state doesn't increment?
            // Let's modify it.

            header.Version = savedVersion; // Reset to what DB has
            // Actually, let's fetch it fresh
            var header2 = await context.PickingListHeaders.FirstAsync(h => h.Id == header.Id);
            header2.Version = 999; // Simulate concurrent update? No, that's not how we test optimistic concurrency.
            // We want to see if version increments on save.

            // Update
            // We need to detach or use a new context to simulate standard app flow, but let's just track it here.
            header.Version = savedVersion; // ensure local tracks

            // Let's just modify the entity
            // We need a non-Version property to modify to trigger State=Modified?
            // PickingListHeader ONLY has Id and Version in stub.
            // Creating a dummy modification might be hard if no other props.
            // But we can force state.

            context.Entry(header).State = EntityState.Modified;
            await context.SaveChangesAsync();

            Assert.Equal(savedVersion + 1, header.Version);
        }
    }
}
