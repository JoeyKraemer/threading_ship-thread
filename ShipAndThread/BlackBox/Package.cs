using ShipAndThread.BlackBox;

namespace BlackBox;

public class Package
{
    public int Id { get; set; }
    public string DeliveryAddress { get; set; }
    public string Status { get; set; }
    public Coordinates Destination { get; set; }
}
