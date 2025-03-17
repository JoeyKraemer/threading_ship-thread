namespace ShipAndThread.Domain.Entities;

public class LocationUpdate
{
    public int UpdateId { get; set; }
    public int TruckId { get; set; }
    public DateTime Timestamp { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public Truck Truck { get; set; }
}