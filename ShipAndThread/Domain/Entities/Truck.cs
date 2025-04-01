using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipAndThread.Domain.Entities;

public class Truck
{
    public int TruckId { get; set; } // Primary key
    public string? LicensePlate { get; set; }
    public int? Capacity { get; set; }
    public LocationHistory? LocationHistory { get; set; } // Optional

    public ICollection<Cargo> CargoList { get; set; } = new List<Cargo>();
}