namespace ShipAndThread.Domain.Entities;

public class Delivery
{
    public int DeliveryId { get; set; }
    public int CargoId { get; set; }
    public int TruckId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string DeliveryStatus { get; set; } = null!;

    public Cargo Cargo { get; set; } = null!;
    public Truck Truck { get; set; } = null!;
}