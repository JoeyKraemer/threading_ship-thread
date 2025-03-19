using ShipAndThread.Domain.Enums;

namespace ShipAndThread.Domain.Entities;

public class Truck
{
    public int TruckId { get; set; }
    public string LicensePlate { get; set; }
    public int DriverId { get; set; }
    public Location CurrentLocation { get; set; }
    public TruckStatus Status { get; set; } 
    public int? CargoId { get; set; }

    public Driver Driver { get; set; } 
    public Cargo? Cargo { get; set; }
}