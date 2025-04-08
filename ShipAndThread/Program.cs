using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipAndThread.BlackBox;
using ShipAndThread.Components;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Application.Services;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Create a connection string for SQLite that uses a file instead of in-memory
var sqliteConnectionString = "Data Source=ShipAndThread.db";

// Register the DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(sqliteConnectionString), ServiceLifetime.Singleton);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register TruckService for DI
builder.Services.AddScoped<TruckService>();

var app = builder.Build();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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

// Start the truck data generation in a background task
_ = Task.Run(async () => 
{
    try
    {
        await RunTruckDataGeneration(app.Services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in truck data generation: {ex.Message}");
    }
});

app.Run();

static async Task RunTruckDataGeneration(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await AsyncDataGeneration.Go(context);
}