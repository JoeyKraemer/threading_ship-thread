using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipAndThread.Application.Services;
using ShipAndThread.Domain.Entities;
using ShipAndThread.Infrastructure.Persistence;
using Xunit;

public class DatabaseQueueServiceTests : IDisposable
{
    private readonly ServiceProvider _serviceProvider;
    private readonly DatabaseQueueService _databaseQueueService;

    public DatabaseQueueServiceTests()
    {
        var services = new ServiceCollection();

        // Configure in-memory SQLite
        var sqliteConnection = new SqliteConnection("Data Source=:memory:;Cache=Shared");
        sqliteConnection.Open();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(sqliteConnection));

        // Register services
        services.AddScoped<LocationHistoryService>();
        services.AddScoped<TruckService>();
        services.AddSingleton<DatabaseQueueService>();

        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();

        _databaseQueueService = _serviceProvider.GetRequiredService<DatabaseQueueService>();
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }

    // ============================
    // Test Cases
    // ============================

    [Fact]
    public async Task EnqueueWrite_ShouldProcessWriteOperation()
    {
        // Arrange
        var truckId = 1;

        // Act
        _databaseQueueService.EnqueueWrite(async serviceProvider =>
        {
            var truckService = serviceProvider.GetRequiredService<TruckService>();
            var truck = new Truck
            {
                TruckId = truckId,
                LicensePlate = "Truck-1",
                Capacity = 100
            };

            await truckService.AddTruckAsync(truck);
        });

        // Wait for the queue to process
        await Task.Delay(500);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var truckService = scope.ServiceProvider.GetRequiredService<TruckService>();
        var truck = await truckService.GetTruckByIdAsync(truckId);

        Assert.NotNull(truck);
        Assert.Equal("Truck-1", truck.LicensePlate);
    }

    [Fact]
    public async Task ReadAsync_ShouldRetrieveData()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var truckService = scope.ServiceProvider.GetRequiredService<TruckService>();
        var truck = new Truck
        {
            TruckId = 1,
            LicensePlate = "Truck-1",
            Capacity = 100
        };
        await truckService.AddTruckAsync(truck);

        // Act
        var result = await _databaseQueueService.ReadAsync(async serviceProvider =>
        {
            var truckService = serviceProvider.GetRequiredService<TruckService>();
            return await truckService.GetTruckByIdAsync(1);
        });

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Truck-1", result.LicensePlate);
    }

    [Fact]
    public async Task EnqueueWrite_ShouldProcessMultipleWritesSequentially()
    {
        // Arrange
        var tasks = Enumerable.Range(1, 5).Select(i =>
        {
            return Task.Run(() =>
            {
                _databaseQueueService.EnqueueWrite(async serviceProvider =>
                {
                    var truckService = serviceProvider.GetRequiredService<TruckService>();
                    var truck = new Truck
                    {
                        TruckId = i,
                        LicensePlate = $"Truck-{i}",
                        Capacity = 100 + i
                    };

                    await truckService.AddTruckAsync(truck);
                });
            });
        });

        // Act
        await Task.WhenAll(tasks);

        // Wait for the queue to process
        await Task.Delay(1000);

        // Assert
        using var scope = _serviceProvider.CreateScope();
        var truckService = scope.ServiceProvider.GetRequiredService<TruckService>();
        var trucks = await truckService.GetAllTrucksAsync();

        Assert.Equal(5, trucks.Count);
        Assert.Contains(trucks, t => t.LicensePlate == "Truck-1");
        Assert.Contains(trucks, t => t.LicensePlate == "Truck-5");
    }

    [Fact]
    public async Task ReadAsync_ShouldHandleConcurrentReads()
    {
        // Arrange
        using var scope = _serviceProvider.CreateScope();
        var truckService = scope.ServiceProvider.GetRequiredService<TruckService>();
        for (int i = 1; i <= 5; i++)
        {
            var truck = new Truck
            {
                TruckId = i,
                LicensePlate = $"Truck-{i}",
                Capacity = 100 + i
            };
            await truckService.AddTruckAsync(truck);
        }

        // Act
        var tasks = Enumerable.Range(1, 5).Select(i =>
        {
            return _databaseQueueService.ReadAsync(async serviceProvider =>
            {
                var truckService = serviceProvider.GetRequiredService<TruckService>();
                return await truckService.GetTruckByIdAsync(i);
            });
        });

        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(5, results.Length);
        Assert.All(results, r => Assert.NotNull(r));
    }

    [Fact]
    public void GetWriteQueueSize_ShouldReturnCorrectSize()
    {
        // Arrange
        _databaseQueueService.EnqueueWrite(async serviceProvider =>
        {
            var truckService = serviceProvider.GetRequiredService<TruckService>();
            var truck = new Truck
            {
                TruckId = 1,
                LicensePlate = "Truck-1",
                Capacity = 100
            };

            await truckService.AddTruckAsync(truck);
        });

        _databaseQueueService.EnqueueWrite(async serviceProvider =>
        {
            var truckService = serviceProvider.GetRequiredService<TruckService>();
            var truck = new Truck
            {
                TruckId = 2,
                LicensePlate = "Truck-2",
                Capacity = 200
            };

            await truckService.AddTruckAsync(truck);
        });

        // Act
        var queueSize = _databaseQueueService.GetWriteQueueSize();

        // Assert
        Assert.Equal(2, queueSize);
    }
}