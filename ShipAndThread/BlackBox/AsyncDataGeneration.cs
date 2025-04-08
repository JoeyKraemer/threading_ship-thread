using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Domain.Entities;

namespace ShipAndThread.BlackBox
{
    public static class AsyncDataGeneration
    {
        public static async Task Go(AppDbContext context)
        {
            // Create instances of required generators
            var dataGenerator = new DataGenerator();
            var truckDataGenerator = new TruckDataGenerator(context, dataGenerator);
            
            // Configuration for the simulation
            int truckCount = 5;     // Number of trucks to simulate
            int cargoPerTruck = 10; // Number of cargo items per truck
            
            Console.WriteLine("Starting truck simulation...");
            
            // Clear the database before starting a new simulation
            await ClearDatabaseAsync(context);
            
            // Create sample trucks
            await CreateSampleTrucksAsync(context, truckCount);
            
            Console.WriteLine($"Simulating {truckCount} trucks with {cargoPerTruck} cargo items each");
            
            // Run the simulation using our new method
            await truckDataGenerator.RunSimulationAsync(truckCount, cargoPerTruck);
            
            Console.WriteLine("Truck simulation completed!");
        }
        
        /// <summary>
        /// Clears all data from the database
        /// </summary>
        private static async Task ClearDatabaseAsync(AppDbContext context)
        {
            Console.WriteLine("Clearing database...");
            
            // Remove all location histories
            context.LocationHistories.RemoveRange(await context.LocationHistories.ToListAsync());
            
            // Remove all cargoes
            context.Cargoes.RemoveRange(await context.Cargoes.ToListAsync());
            
            // Remove all trucks
            context.Trucks.RemoveRange(await context.Trucks.ToListAsync());
            
            // Save changes
            await context.SaveChangesAsync();
            
            Console.WriteLine("Database cleared successfully.");
        }
        
        /// <summary>
        /// Creates sample trucks in the database
        /// </summary>
        private static async Task CreateSampleTrucksAsync(AppDbContext context, int count)
        {
            Console.WriteLine($"Creating {count} sample trucks...");
            
            // Create sample trucks
            var trucks = new List<Truck>();
            for (int i = 1; i <= count; i++)
            {
                var truck = new Truck
                {
                    TruckId = i,
                    LicensePlate = $"TRUCK-{i:D3}",
                    CargoList = new List<Cargo>()
                };
                
                trucks.Add(truck);
            }
            
            // Add trucks to database
            context.Trucks.AddRange(trucks);
            await context.SaveChangesAsync();
            
            Console.WriteLine($"Created {count} sample trucks in the database.");
        }
    }
}