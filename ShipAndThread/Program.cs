using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipAndThread.BlackBox;
using ShipAndThread.Components;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Application.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// Create a connection string for SQLite that uses a file instead of in-memory
var sqliteConnectionString = "Data Source=ShipAndThread.db";

// Register the DbContext with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(sqliteConnectionString), ServiceLifetime.Singleton);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register services for DI with Singleton lifetime to match DbContext
builder.Services.AddSingleton<TruckService>();
builder.Services.AddSingleton<CargoService>();
builder.Services.AddSingleton<LocationHistoryService>();

builder.Services.AddScoped<ITruckDataSimulationService, TruckDataSimulationService>();

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<CommunicationHub>("/communicationHub");

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

app.Run();
