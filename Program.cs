using CMetalsFulfillment.Components;
using CMetalsFulfillment.Components.Account;
using CMetalsFulfillment.Data;
using CMetalsFulfillment.Services;
using CMetalsFulfillment.Services.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register Factory with Singleton lifetime (explicitly) and ensure Options are Singleton/Scoped correctly?
// Actually, AddDbContextFactory defaults to Singleton. It needs DbContextOptions which are normally Scoped if added via AddDbContext.
// We should configure options in the factory builder.
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Register Scoped DbContext for Identity using the Factory
builder.Services.AddScoped<ApplicationDbContext>(p =>
    p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Register Application Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBranchContext, BranchContext>();
builder.Services.AddScoped<IRoleResolver, RoleResolver>();
builder.Services.AddScoped<ISetupStatusService, SetupStatusService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// Authorization
builder.Services.AddScoped<IClaimsTransformation, BranchClaimsTransformation>();
builder.Services.AddScoped<IAuthorizationHandler, BranchRoleHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", p => p.AddRequirements(new BranchRoleRequirement("BranchAdmin")));
    options.AddPolicy("PlannerPolicy", p => p.AddRequirements(new BranchRoleRequirement("Planner", "Supervisor", "BranchAdmin")));
    options.AddPolicy("OperatorPolicy", p => p.AddRequirements(new BranchRoleRequirement("Operator", "Supervisor", "BranchAdmin")));
    options.AddPolicy("LoaderCheckerPolicy", p => p.AddRequirements(new BranchRoleRequirement("LoaderChecker", "Supervisor", "BranchAdmin")));
    options.AddPolicy("DriverPolicy", p => p.AddRequirements(new BranchRoleRequirement("Driver", "Supervisor", "BranchAdmin")));
    options.AddPolicy("ViewerPolicy", p => p.AddRequirements(new BranchRoleRequirement("Viewer", "Driver", "LoaderChecker", "Operator", "Planner", "Supervisor", "BranchAdmin")));
});

var app = builder.Build();

// Migrate Database & Seed
using (var scope = app.Services.CreateScope())
{
    // Make sure to apply migrations first
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    // Seed Data
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
