using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Domain.Entities;
using ShipAndThread.Application.Services;

namespace ShipAndThread.BlackBox
{
    public static class AsyncDataGeneration
    {
        public static async Task Go(
            AppDbContext context, 
            IHubContext<CommunicationHub> hubContext,
            TruckService truckService,
            CargoService cargoService,
            LocationHistoryService locationHistoryService)
        {
            // Create instances of required generators
            var dataGenerator = new DataGenerator();
            var truckDataGenerator = new TruckDataGenerator(
                context, 
                dataGenerator, 
                hubContext, 
                truckService, 
                cargoService, 
                locationHistoryService);
            
            // Configuration for the simulation
            int truckCount = 5;     // Number of trucks to simulate
            int cargoPerTruck = 10; // Number of cargo items per truck
            
            Console.WriteLine("Starting truck simulation...");
            
            // Clear the database before starting a new simulation
            await ClearDatabaseAsync(truckService, cargoService, locationHistoryService);
            
            // Create sample trucks
            await CreateSampleTrucksAsync(truckService, truckCount);
            
            Console.WriteLine($"Simulating {truckCount} trucks with {cargoPerTruck} cargo items each");
            
            // Run the simulation using our new method
            await truckDataGenerator.RunSimulationAsync(truckCount, cargoPerTruck);
            
            Console.WriteLine("Truck simulation completed!");
        }
        
        /// <summary>
        /// Clears all data from the database using service classes
        /// </summary>
        private static async Task ClearDatabaseAsync(
            TruckService truckService, 
            CargoService cargoService, 
            LocationHistoryService locationHistoryService)
        {
            Console.WriteLine("Clearing database...");
            
            // Get all location histories and remove them
            var locationHistories = await locationHistoryService.GetAllLocationHistoriesAsync();
            foreach (var history in locationHistories)
            {
                await locationHistoryService.DeleteLocationHistoryAsync(history.Id);
            }
            
            // Get all cargoes and remove them
            var cargoes = await cargoService.GetAllCargoAsync();
            foreach (var cargo in cargoes)
            {
                await cargoService.DeleteCargoAsync(cargo.Id);
            }
            
            // Get all trucks and remove them
            var trucks = await truckService.GetAllTrucksAsync();
            foreach (var truck in trucks)
            {
                await truckService.DeleteTruckAsync(truck.TruckId);
            }
            
            Console.WriteLine("Database cleared successfully.");
        }
        
        /// <summary>
        /// Creates sample trucks in the database using TruckService
        /// </summary>
        private static async Task CreateSampleTrucksAsync(TruckService truckService, int count)
        {
            Console.WriteLine($"Creating {count} sample trucks...");
            
            // Create sample trucks
            for (int i = 1; i <= count; i++)
            {
                var truck = new Truck
                {
                    TruckId = i,
                    LicensePlate = $"TRUCK-{i:D3}",
                    CargoList = new List<Cargo>()
                };
                
                await truckService.AddTruckAsync(truck);
            }
            
            Console.WriteLine($"Created {count} sample trucks in the database.");
        }
    }
}
