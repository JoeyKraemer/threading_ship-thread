using ShipAndThread.Domain.Enums;

namespace ShipAndThread.Domain.Entities;

public class Location
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public TruckStatus Status { get; set; }
}