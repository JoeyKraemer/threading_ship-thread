namespace ShipAndThread.Domain.Entities;

public class Event
{
    public int EventId { get; set; }
    public int TruckId { get; set; }
    public string EventType { get; set; }
    public string Description { get; set; }
    public DateTime Timestamp { get; set; }

    public Truck Truck { get; set; }
}