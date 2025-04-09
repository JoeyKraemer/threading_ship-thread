using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Domain.Entities;
using ShipAndThread.Domain.Enums;
using ShipAndThread.Application.Services;

namespace ShipAndThread.BlackBox
{
    public class TruckDataGenerator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DataGenerator _dataGenerator;
        private readonly ArrayList _trucks;
        private readonly ArrayList _cargoes;
        private readonly IHubContext<CommunicationHub> _hubContext;

        public TruckDataGenerator(
            IServiceProvider serviceProvider, 
            DataGenerator dataGenerator, 
            IHubContext<CommunicationHub> hubContext)
        {
            _serviceProvider = serviceProvider;
            _dataGenerator = dataGenerator;
            _hubContext = hubContext;
            _trucks = new ArrayList();
            _cargoes = new ArrayList();
        }

        /// <summary>
        /// Initializes the list of trucks by fetching them from the database
        /// and adding them to the _trucks ArrayList. Limits the number of trucks
        /// to the specified truckCount.
        /// </summary>
        public async Task InitializeTrucksAsync(int truckCount)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var truckService = scope.ServiceProvider.GetRequiredService<TruckService>();
                var trucks = await truckService.GetAllTrucksAsync();
            if (trucks.Count > truckCount)
                trucks = trucks.Take(truckCount).ToList();

            foreach (var truck in trucks)
            {
                _trucks.Add(truck);
            }
        }
        }

        /// <summary>
        /// Assigns a specified number of cargo items to a given truck.
        /// Generates cargo with unique IDs and random status.
        /// </summary>
        public async Task AssignCargoToTruck(Truck truck, int cargoCount)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cargoService = scope.ServiceProvider.GetRequiredService<CargoService>();
                for (int i = 0; i < cargoCount; i++)
                {
                var cargo = new Cargo
                {
                    Id = _cargoes.Count + 1,
                    TruckId = truck.TruckId,
                    Status = CargoStatus.OnRoute,
                };

                truck.CargoList.Add(cargo);
                _cargoes.Add(cargo);
                
                // Add cargo to the database using the service
                    await cargoService.AddCargoAsync(cargo);
                }
            }
        }
        
        /// <summary>
        /// Simulates the truck driving along a generated route and dropping off cargo.
        /// The truck stops at each destination in the route, drops off one cargo item,
        /// and proceeds to the next destination after a random interval between 2-12 seconds.
        /// </summary>
        public async Task SimulateTruckRouteAsync(Truck truck)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var truckService = scope.ServiceProvider.GetRequiredService<TruckService>();
                var cargoService = scope.ServiceProvider.GetRequiredService<CargoService>();
                var locationHistoryService = scope.ServiceProvider.GetRequiredService<LocationHistoryService>();
            var route = GenerateRoute(truck.CargoList.Count);
            var random = new Random();

            foreach (var destination in route)
            {
                // Generate a random interval between 1000ms (1seconds) and 3000ms (3 seconds)
                int randomIntervalMs = random.Next(1000, 3001);
                    await DriveToDestination(truck, destination, randomIntervalMs, locationHistoryService);
                    await DropOffCargo(truck, destination, cargoService);
                }
            }
        }

        /// <summary>
        /// Generates a list of random geographical coordinates to represent
        /// the route that the truck will follow. The number of stops is determined
        /// by the number of cargo items.
        /// </summary>
        private List<(double Latitude, double Longitude)> GenerateRoute(int stops)
        {
            var route = new List<(double Latitude, double Longitude)>();

            for (int i = 0; i < stops; i++)
            {
                route.Add(_dataGenerator.GenerateCoordinates());
            }

            return route;
        }

        /// <summary>
        /// Simulates the truck driving to a specified destination. Adds a delay
        /// to mimic the travel time and saves the truck's location history to the database.
        /// </summary>
        private async Task DriveToDestination(
            Truck truck, 
            (double Latitude, double Longitude) destination, 
            int interval,
            LocationHistoryService locationHistoryService)
        {
            await Task.Delay(interval);

            // Create location history
            var locationHistory = new LocationHistory
            {
                TruckId = truck.TruckId,
                Truck = truck, // Set the Truck property
                Latitude = destination.Latitude,
                Longitude = destination.Longitude,
                Timestamp = _dataGenerator.GenerateTimestamp()
            };

            // Add location history using the service
            await locationHistoryService.AddLocationHistoryAsync(locationHistory);
            
            // Send the new location to all connected clients via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveLocationUpdate", new
            {
                truckId = truck.TruckId,
                latitude = locationHistory.Latitude,
                longitude = locationHistory.Longitude,
                timestamp = locationHistory.Timestamp
            });
        }

        /// <summary>
        /// Simulates dropping off a cargo item at the current destination.
        /// Updates the cargo status to Delivered but keeps it in the database.
        /// </summary>
        private async Task DropOffCargo(
            Truck truck, 
            (double Latitude, double Longitude) destination,
            CargoService cargoService)
        {
            // Find the first cargo that is not already delivered
            var cargo = truck.CargoList.FirstOrDefault(c => c.Status != CargoStatus.Delivered);
            
            if (cargo != null)
            {
                // Update cargo status instead of removing it
                cargo.Status = CargoStatus.Delivered;
                
                // Update the cargo in the database using the service
                await cargoService.UpdateCargoAsync(cargo);
                
                // Remove from local tracking only, but keep in the truck's cargo list
                _cargoes.Remove(cargo);

                // Print information about the truck and cargo
                Console.WriteLine($"Truck {truck.TruckId} dropped off cargo at {destination.Latitude}, {destination.Longitude} with status {cargo.Status}");
                
                // Broadcast updated active cargo list
                await SendActiveCargoUpdateAsync();
            }
        }
        
        private async Task SendActiveCargoUpdateAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cargoService = scope.ServiceProvider.GetRequiredService<CargoService>();
            // Get all cargo items, including delivered ones
                var allCargo = await cargoService.GetAllCargoAsync();
            var cargoList = new List<object>();

            foreach (var cargo in allCargo)
            {
                // Create a simplified DTO without circular references
                var cargoDto = new
                {
                    Id = cargo.Id,
                    TruckId = cargo.TruckId,
                    Status = cargo.Status,
                    Truck = cargo.Truck != null ? new
                    {
                        TruckId = cargo.Truck.TruckId,
                        LicensePlate = cargo.Truck.LicensePlate,
                        Capacity = cargo.Truck.Capacity
                        // Omit LocationHistory and CargoList to avoid circular references
                    } : null
                };
                
                cargoList.Add(cargoDto);
            }

            await _hubContext.Clients.All.SendAsync("ReceiveCargoUpdate", cargoList);
        }
        }
        
        /// <summary>
        /// Runs the entire truck simulation process, including initializing trucks, assigning cargo,
        /// and simulating their routes. This method orchestrates the entire simulation workflow
        /// from start to finish, connecting all the individual simulation components.
        /// </summary>
        public async Task RunSimulationAsync(int truckCount, int cargoPerTruck)
        {
            try
            {
                // Step 1: Initialize trucks from the database
                await InitializeTrucksAsync(truckCount);
                Console.WriteLine($"Initialized {_trucks.Count} trucks for simulation");

                // Step 2: Assign cargo to each truck
                foreach (Truck truck in _trucks)
                {
                    await AssignCargoToTruck(truck, cargoPerTruck);
                    Console.WriteLine($"Assigned {cargoPerTruck} cargo items to Truck {truck.TruckId}");
                }

                // Step 3: Simulate each truck's route using ThreadPool for parallel execution
                Console.WriteLine("Starting route simulations using ThreadPool...");
                
                // Create a semaphore to limit concurrent threads (adjust based on system capabilities)
                var maxConcurrentThreads = Math.Min(_trucks.Count, Environment.ProcessorCount * 2);
                var semaphore = new SemaphoreSlim(maxConcurrentThreads);
                
                // Create tasks for each truck simulation
                var simulationTasks = new List<Task>();
                
                foreach (Truck truck in _trucks)
                {
                    // Capture the truck in a local variable to avoid closure issues
                    var truckToSimulate = truck;
                    
                    // Create a task that will use the ThreadPool
                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            // Acquire a slot in the semaphore
                            await semaphore.WaitAsync();
                            
                            Console.WriteLine($"ThreadPool worker starting simulation for Truck {truckToSimulate.TruckId}");
                            await SimulateTruckRouteAsync(truckToSimulate);
                            Console.WriteLine($"ThreadPool worker completed simulation for Truck {truckToSimulate.TruckId}");
                        }
                        finally
                        {
                            // Release the semaphore slot
                            semaphore.Release();
                        }
                    });
                    
                    simulationTasks.Add(task);
                }
                
                // Wait for all simulations to complete asynchronously
                await Task.WhenAll(simulationTasks);
                
                Console.WriteLine("All truck simulations completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in truck simulation: {ex.Message}");
                throw;
            }
        }
    }
}