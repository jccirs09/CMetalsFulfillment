using MudBlazor.Services;
using CMetalsFulfillment.Components;
using CMetalsFulfillment.Components.Account;
using CMetalsFulfillment.Data;
using CMetalsFulfillment.Features.Auth;
using CMetalsFulfillment.Features.Admin;
using CMetalsFulfillment.Features.Config;
using CMetalsFulfillment.Features.Items;
using CMetalsFulfillment.Features.Inventory;
using CMetalsFulfillment.Features.PickingLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBranchContext, BranchContext>();
builder.Services.AddScoped<SetupGateService>();
builder.Services.AddScoped<IAuthorizationHandler, BranchPermissionHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthConstants.PolicyBranchAccess, policy =>
        policy.Requirements.Add(new BranchRoleRequirement()));

    options.AddPolicy(AuthConstants.PolicyCanAdminBranch, policy =>
        policy.Requirements.Add(new BranchRoleRequirement(AuthConstants.RoleBranchAdmin)));

    options.AddPolicy(AuthConstants.PolicyCanPlan, policy =>
        policy.Requirements.Add(new BranchRoleRequirement(AuthConstants.RoleBranchAdmin, AuthConstants.RolePlanner, AuthConstants.RoleSupervisor)));

    options.AddPolicy(AuthConstants.PolicyCanExecuteProduction, policy =>
        policy.Requirements.Add(new BranchRoleRequirement(AuthConstants.RoleBranchAdmin, AuthConstants.RoleSupervisor, AuthConstants.RoleOperator)));

    options.AddPolicy(AuthConstants.PolicyCanVerifyLoad, policy =>
        policy.Requirements.Add(new BranchRoleRequirement(AuthConstants.RoleBranchAdmin, AuthConstants.RoleSupervisor, AuthConstants.RoleLoaderChecker)));

    options.AddPolicy(AuthConstants.PolicyCanVerifyDelivery, policy =>
        policy.Requirements.Add(new BranchRoleRequirement(AuthConstants.RoleBranchAdmin, AuthConstants.RoleSupervisor, AuthConstants.RoleDriver)));
});

var app = builder.Build();

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
app.MapAuthEndpoints();
app.MapAdminEndpoints();
app.MapConfigEndpoints();
app.MapItemEndpoints();
app.MapInventoryEndpoints();
app.MapPickingListEndpoints();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    if (context.Database.IsRelational())
    {
        context.Database.Migrate();
    }
}

await CMetalsFulfillment.Data.DbSeeder.SeedAsync(app.Services);

app.Run();
