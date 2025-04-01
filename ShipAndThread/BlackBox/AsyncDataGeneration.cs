using BlackBox;

namespace ShipAndThread.BlackBox
{
    public static class AsyncDataGeneration
    {
        public static async Task Go()
        {
            int interval = 5000;  // 5 seconds
            var trucks = new List<Truck>
            {
                new Truck { Id = 1, Packages = new List<Package> { new Package { Id = 1, DeliveryAddress = "Address 1" }, new Package { Id = 2, DeliveryAddress = "Address 2" } } },
                new Truck { Id = 2, Packages = new List<Package> { new Package { Id = 3, DeliveryAddress = "Address 3" }, new Package { Id = 4, DeliveryAddress = "Address 4" } } }
            };

            var generator = new TruckDataGenerator();
            await generator.GenerateTruckDataAsync(interval, trucks);
        }
    }
}