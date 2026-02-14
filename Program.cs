using CMetalsFulfillment.Components;
using CMetalsFulfillment.Components.Account;
using CMetalsFulfillment.Data;
using CMetalsFulfillment.Services;
using CMetalsFulfillment.Services.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMudServices();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// App Services
builder.Services.AddScoped<IBranchContext, BranchContext>();
builder.Services.AddScoped<IRoleResolver, RoleResolver>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<BranchService>();
builder.Services.AddScoped<SetupStatusService>();
builder.Services.AddScoped<SeedService>();
builder.Services.AddScoped<CookieService>();

builder.Services.AddScoped<IClaimsTransformation, BranchClaimsTransformation>();
builder.Services.AddScoped<IAuthorizationHandler, BranchRoleHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.AddRequirements(new BranchRoleRequirement("BranchAdmin")));
    options.AddPolicy("PlannerPolicy", policy => policy.AddRequirements(new BranchRoleRequirement("Planner")));
    options.AddPolicy("OperatorPolicy", policy => policy.AddRequirements(new BranchRoleRequirement("Operator")));
    options.AddPolicy("LoaderCheckerPolicy", policy => policy.AddRequirements(new BranchRoleRequirement("LoaderChecker")));
    options.AddPolicy("DriverPolicy", policy => policy.AddRequirements(new BranchRoleRequirement("Driver")));
    options.AddPolicy("ViewerPolicy", policy => policy.AddRequirements(new BranchRoleRequirement("Viewer")));
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));
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

var app = builder.Build();

// Seed Data
using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
    await seedService.InitializeAsync();
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
