using System.ComponentModel.DataAnnotations.Schema;

namespace ShipAndThread.Domain.Entities;

public class CargoList
{
    public int CargoId { get; set; }
    public int TruckId { get; set; }
        
    [ForeignKey("CargoId")]
    public Cargo Cargo { get; set; }
        
    [ForeignKey("TruckId")]
    public Truck Truck { get; set; }
}