using System.ComponentModel.DataAnnotations;

namespace ShipAndThread.Domain.Entities;

public class Truck
{
    [Key]
    public int Id { get; set; }
        
    [Required]
    public string LicensePlate { get; set; }
        
    public int Capacity { get; set; }
        
    public ICollection<LocationHistory> LocationHistories { get; set; }
    public ICollection<CargoList> CargoLists { get; set; }
}