using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipAndThread.Domain.Entities;

public class LocationHistory
{
    public int Id { get; set; }
    public required int TruckId { get; set; } // Foreign key
    public required Truck Truck { get; set; } // Navigation property

    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; }
}