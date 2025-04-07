using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipAndThread.BlackBox;
using ShipAndThread.Components;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Application.Services;



var builder = WebApplication.CreateBuilder(args);

// Create and open the in-memory SQLite connection
var sqliteConnectionString = "Data Source=:memory:;Cache=Shared";

// Register the DbContext with the in-memory SQLite connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(sqliteConnectionString), ServiceLifetime.Scoped);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register TruckService for DI
builder.Services.AddScoped<TruckDataProcessor>();

builder.Services.AddDbContext<LogisticsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.OpenConnection(); // Open the connection
    dbContext.Database.EnsureCreated();  // Create the schema
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

_ = RunTruckDataGeneration();

app.Run();

static async Task RunTruckDataGeneration()
{
    await AsyncDataGeneration.Go();
}
