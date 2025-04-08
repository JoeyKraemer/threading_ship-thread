using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Domain.Entities;
using ShipAndThread.Domain.Enums;

namespace ShipAndThread.BlackBox
{
    public class TruckDataGenerator
    {
        private readonly AppDbContext _context;
        private readonly DataGenerator _dataGenerator;
        private readonly ArrayList _trucks;
        private readonly ArrayList _cargoes;
        private readonly IHubContext<CommunicationHub> _hubContext;

        public TruckDataGenerator(AppDbContext context, DataGenerator dataGenerator, IHubContext<CommunicationHub> hubContext)
        {
            _context = context;
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
            var trucks = await _context.Trucks.ToListAsync();
            if (trucks.Count > truckCount)
                trucks = trucks.Take(truckCount).ToList();

            foreach (var truck in trucks)
            {
                _trucks.Add(truck);
            }
        }

        /// <summary>
        /// Assigns a specified number of cargo items to a given truck.
        /// Generates cargo with unique IDs and random status.
        /// </summary>
        public async Task AssignCargoToTruck(Truck truck, int cargoCount)
        {
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
                
                // Add cargo to the database
                _context.Cargoes.Add(cargo);
            }
            
            // Save all changes to the database
            await _context.SaveChangesAsync();
        }
        
        /// <summary>
        /// Simulates the truck driving along a generated route and dropping off cargo.
        /// The truck stops at each destination in the route, drops off one cargo item,
        /// and proceeds to the next destination after a random interval between 2-12 seconds.
        /// </summary>
        public async Task SimulateTruckRouteAsync(Truck truck)
        {
            var route = GenerateRoute(truck.CargoList.Count);
            var random = new Random();

            foreach (var destination in route)
            {
                // Generate a random interval between 2000ms (2 seconds) and 12000ms (12 seconds)
                int randomIntervalMs = random.Next(2000, 12001);
                await DriveToDestination(truck, destination, randomIntervalMs);
                await DropOffCargo(truck, destination);
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
        private async Task DriveToDestination(Truck truck, (double Latitude, double Longitude) destination, int interval)
        {
            // Simulate driving to the destination
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

            _context.LocationHistories.Add(locationHistory);
            await _context.SaveChangesAsync();
            
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
        /// Removes the cargo from the truck's cargo list and prints information
        /// about the drop-off.
        /// </summary>
        private async Task DropOffCargo(Truck truck, (double Latitude, double Longitude) destination)
        {
            if (truck.CargoList.Any())
            {
                var cargo = truck.CargoList.First();
                truck.CargoList.Remove(cargo);
                cargo.Status = CargoStatus.Delivered;
                _cargoes.Remove(cargo);

                // Print information about the truck and cargo
                Console.WriteLine($"Truck {truck.TruckId} dropped off cargo at {destination.Latitude}, {destination.Longitude} with status {cargo.Status}");
                
                // Broadcast updated active cargo list
                await SendActiveCargoUpdateAsync();
            }
        }
        
        private async Task SendActiveCargoUpdateAsync()
        {
            var activeCargo = await _context.Cargoes
                .Where(c => c.Status != CargoStatus.Delivered)
                .ToListAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveCargoUpdate", activeCargo);
        }
        
        /// <summary>
        /// Runs the complete truck simulation process by initializing trucks, assigning cargo,
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

                // Step 3: Simulate each truck's route with random intervals
                var simulationTasks = new List<Task>();
                
                foreach (Truck truck in _trucks)
                {
                    Console.WriteLine($"Starting route simulation for Truck {truck.TruckId}");
                    simulationTasks.Add(SimulateTruckRouteAsync(truck));
                }

                // Wait for all truck simulations to complete
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