namespace ShipAndThread.Domain.Entities;

public class Driver
{
    public int DriverId { get; set; }
    public string Name { get; set; } = null!;
    public string ContactNumber { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string LicenseType { get; set; } = null!;
}