using CMetalsFulfillment.Components;
using CMetalsFulfillment.Components.Account;
using CMetalsFulfillment.Data;
using CMetalsFulfillment.Features.Admin;
using CMetalsFulfillment.Features.Auth;
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

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBranchContext, BranchContext>();
builder.Services.AddScoped<IAuthorizationHandler, BranchAccessHandler>();
builder.Services.AddScoped<IAuthorizationHandler, BranchRoleHandler>();
builder.Services.AddScoped<SetupStatusService>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BranchAccessPolicy", policy => policy.Requirements.Add(new BranchAccessRequirement()));
    options.AddPolicy("CanAdminBranch", policy => policy.Requirements.Add(new BranchRoleRequirement("BranchAdmin")));
    options.AddPolicy("CanPlan", policy => policy.Requirements.Add(new BranchRoleRequirement("Planner", "Supervisor", "BranchAdmin")));
    options.AddPolicy("CanExecuteProduction", policy => policy.Requirements.Add(new BranchRoleRequirement("Operator", "Supervisor")));
    options.AddPolicy("CanVerifyLoad", policy => policy.Requirements.Add(new BranchRoleRequirement("LoaderChecker", "Supervisor")));
    options.AddPolicy("CanVerifyDelivery", policy => policy.Requirements.Add(new BranchRoleRequirement("Driver", "Supervisor")));
});

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
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders()
    .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

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

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (app.Environment.IsDevelopment())
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
    else
    {
        await context.Database.MigrateAsync();
    }

    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
