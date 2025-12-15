using Microsoft.EntityFrameworkCore;
using SkiServiceLogbook.Web.Components;
using SkiServiceLogbook.Infrastructure.Data;
using SkiServiceLogbook.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<SkiServiceDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=skiservice.db",
        b => b.MigrationsAssembly("SkiServiceLogbook.Infrastructure")));

// Add Cascading authentication state
builder.Services.AddCascadingAuthenticationState();

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanManageInventory", policy =>
        policy.RequireRole(Role.Admin.ToString()));
    
    options.AddPolicy("CanCreateTests", policy =>
        policy.RequireRole(Role.Admin.ToString(), Role.Huolto.ToString()));
    
    options.AddPolicy("CanEnterResults", policy =>
        policy.RequireRole(Role.Admin.ToString(), Role.Huolto.ToString(), Role.Urheilija.ToString()));
    
    options.AddPolicy("ReadOnlyReports", policy =>
        policy.RequireAuthenticatedUser());
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
