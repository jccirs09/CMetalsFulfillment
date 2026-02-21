using CMetalsFulfillment.Components;
using CMetalsFulfillment.Components.Account;
using CMetalsFulfillment.Data;
using CMetalsFulfillment.Services;
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
builder.Services.AddHttpContextAccessor();

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

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Register DbContext as Scoped (using Factory) for Identity
builder.Services.AddScoped(p => p.GetRequiredService<IDbContextFactory<ApplicationDbContext>>().CreateDbContext());

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
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IUserBranchService, UserBranchService>();
builder.Services.AddScoped<IAuditEventService, AuditEventService>();
builder.Services.AddScoped<IRoleResolver, RoleResolver>();
builder.Services.AddScoped<IBranchContext, BranchContext>();
builder.Services.AddScoped<ISetupStatusService, SetupStatusService>();

// Auth Services
builder.Services.AddScoped<IClaimsTransformation, BranchClaimsTransformation>();
builder.Services.AddScoped<IAuthorizationHandler, BranchAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SystemAdmin", policy => policy.RequireRole("SystemAdmin"));

    options.AddPolicy("BranchAdmin", policy =>
        policy.AddRequirements(new BranchRoleRequirement("BranchAdmin")));

    options.AddPolicy("Supervisor", policy =>
        policy.AddRequirements(new BranchRoleRequirement("Supervisor", "BranchAdmin")));

    options.AddPolicy("Planner", policy =>
        policy.AddRequirements(new BranchRoleRequirement("Planner", "BranchAdmin")));

    options.AddPolicy("Operator", policy =>
        policy.AddRequirements(new BranchRoleRequirement("Operator", "BranchAdmin")));

    options.AddPolicy("LoaderChecker", policy =>
        policy.AddRequirements(new BranchRoleRequirement("LoaderChecker", "BranchAdmin")));

    options.AddPolicy("Driver", policy =>
        policy.AddRequirements(new BranchRoleRequirement("Driver", "BranchAdmin")));

    options.AddPolicy("Viewer", policy =>
        policy.AddRequirements(new BranchRoleRequirement(
            "Viewer", "Driver", "LoaderChecker", "Operator", "Planner", "Supervisor", "BranchAdmin")));
});

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await Seeder.SeedAsync(services, app.Configuration);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
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
