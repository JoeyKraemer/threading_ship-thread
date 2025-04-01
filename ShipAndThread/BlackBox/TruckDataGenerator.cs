using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using BlackBox;

namespace ShipAndThread.BlackBox
{
    public class TruckDataGenerator
    {
        private DataGenerator dataGenerator = new DataGenerator();

        public async Task GenerateTruckDataAsync(int interval, List<Truck> trucks)
        {
            int routeIndex = 0;
            var route = new List<(double Latitude, double Longitude)>
            {
                (37.7749, -122.4194),  // Start point (San Francisco)
                (37.7849, -122.4094),
                (37.7949, -122.3994),
                (37.8049, -122.3894),
                (37.8149, -122.3794)   // End point
            };

            while (routeIndex < route.Count)
            {
                foreach (var truck in trucks)
                {
                    var (latitude, longitude) = route[routeIndex];
                    var timestamp = dataGenerator.GenerateTimestamp();
                    var status = dataGenerator.GenerateStatus();

                    var data = new TruckData()
                    {
                        Truck = new Location
                        {
                            Coordinates = new Coordinates { Latitude = latitude, Longitude = longitude },
                            Timestamp = timestamp
                        },
                        Package = new Package
                        {
                            Status = status,
                            Destination = new Coordinates { Latitude = latitude + 0.001, Longitude = longitude + 0.001 }
                        }
                    };

                    var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                    Console.WriteLine(json);
                }

                routeIndex++;
                await Task.Delay(interval);
            }
        }
    }
}