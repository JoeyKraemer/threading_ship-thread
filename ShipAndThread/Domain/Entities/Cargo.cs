namespace ShipAndThread.Domain.Entities;

public class Cargo
{
    public int CargoId { get; set; }
    public string Description { get; set; }
    public double Weight { get; set; }
    public string Destination { get; set; }
    public string Status { get; set; }
}