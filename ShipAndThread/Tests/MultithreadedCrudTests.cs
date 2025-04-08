using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore; // Add this namespace
using ShipAndThread.Application.Services;
using ShipAndThread.Domain.Entities;
using ShipAndThread.Infrastructure.Persistence;
using Xunit;

[CollectionDefinition("TestCollection")]
public class TestCollection : ICollectionFixture<TestFixture> { }

[Collection("TestCollection")]
public class MultithreadedCrudTests
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

    public MultithreadedCrudTests(TestFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
    }

    [Fact]
    public async Task TestSingleTruckLocationHistory()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Inspect the database schema
        await InspectDatabaseSchema(dbContext);

        Console.WriteLine("Creating a new truck...");
        var truck = new Truck
        {
            TruckId = Guid.NewGuid().GetHashCode(),
            LicensePlate = "Truck-1",
            Capacity = 100
        };

        dbContext.Trucks.Add(truck);
        await dbContext.SaveChangesAsync();
        Console.WriteLine($"Truck created with ID: {truck.TruckId}");

        Console.WriteLine("Creating a new location history...");
        var locationHistory = new LocationHistory
        {
            TruckId = truck.TruckId,
            Truck = truck,
            Latitude = 0.0,
            Longitude = 0.0,
            Timestamp = DateTime.UtcNow
        };

        dbContext.LocationHistories.Add(locationHistory);
        await dbContext.SaveChangesAsync();
        Console.WriteLine($"LocationHistory created with ID: {locationHistory.Id}, Latitude: {locationHistory.Latitude}, Longitude: {locationHistory.Longitude}");

        Console.WriteLine("Updating location history...");
        locationHistory.Latitude += 1.0;
        locationHistory.Longitude += 1.0;
        Console.WriteLine($"Entity State Before Update: {dbContext.Entry(locationHistory).State}");
        dbContext.LocationHistories.Update(locationHistory);
        Console.WriteLine($"Entity State After Update: {dbContext.Entry(locationHistory).State}");
        await dbContext.SaveChangesAsync();

        Console.WriteLine("Reloading location history...");
        await dbContext.Entry(locationHistory).ReloadAsync();
        Console.WriteLine($"Reloaded LocationHistory: Latitude = {locationHistory.Latitude}, Longitude = {locationHistory.Longitude}");

        // Corrected assertions
        Assert.Equal(1.0, locationHistory.Latitude);
        Assert.Equal(1.0, locationHistory.Longitude);
    }

    public async Task InspectDatabaseSchema(AppDbContext dbContext)
    {
        Console.WriteLine("Inspecting database schema for LocationHistories table...");
        var schemaInfo = await dbContext.Database.ExecuteSqlRawAsync("PRAGMA table_info(LocationHistories);");

        using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "PRAGMA table_info(LocationHistories);";

        dbContext.Database.OpenConnection();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            Console.WriteLine($"Column: {reader["name"]}, Type: {reader["type"]}, NotNull: {reader["notnull"]}, Default: {reader["dflt_value"]}");
        }

        dbContext.Database.CloseConnection();
    }
}