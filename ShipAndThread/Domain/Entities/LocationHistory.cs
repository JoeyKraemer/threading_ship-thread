using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShipAndThread.Domain.Entities;

public class LocationHistory
{
    [Key]
    public int Id { get; set; }
        
    public int TruckId { get; set; }
        
    [Required]
    public DateTime Timestamp { get; set; }
        
    public double Latitude { get; set; }
    public double Longitude { get; set; }
        
    [ForeignKey("TruckId")]
    public Truck Truck { get; set; }
    
}