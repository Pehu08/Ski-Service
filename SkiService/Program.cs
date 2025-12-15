using Microsoft.EntityFrameworkCore;
using SkiServiceLogbook.Infrastructure.Data;
using SkiServiceLogbook.Domain.Enums;
using SkiServiceLogbook.Services;
using SkiServiceLogbook.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<SkiServiceDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=skiservice.db",
        b => b.MigrationsAssembly("SkiServiceLogbook.Infrastructure")));

// Register custom services
builder.Services.AddScoped<BracketService>();
builder.Services.AddScoped<SearchService>();

// Lisää valtuutuskäytännöt
builder.Services.AddAuthorization(options =>
{
    // CanManageInventory - Vain Admin voi hallita varastoa
    options.AddPolicy(AuthorizationPolicies.CanManageInventory, policy =>
        policy.RequireRole(Role.Admin.ToString()));
    
    // CanCreateTests - Admin ja Huolto voivat luoda testejä
    options.AddPolicy(AuthorizationPolicies.CanCreateTests, policy =>
        policy.RequireRole(Role.Admin.ToString(), Role.Huolto.ToString()));
    
    // CanEnterResults - Admin, Huolto ja Urheilija voivat syöttää tuloksia
    options.AddPolicy(AuthorizationPolicies.CanEnterResults, policy =>
        policy.RequireRole(Role.Admin.ToString(), Role.Huolto.ToString(), Role.Urheilija.ToString()));
    
    // ReadOnlyReports - Kaikki autentikoidut käyttäjät voivat lukea raportteja
    options.AddPolicy(AuthorizationPolicies.ReadOnlyReports, policy =>
        policy.RequireAuthenticatedUser());
});

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Ski Service Logbook API", 
        Version = "v1",
        Description = "API for managing ski waxing and testing logbook"
    });
});

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SkiServiceDbContext>();
    context.Database.Migrate();
    await SeedData.InitializeAsync(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
