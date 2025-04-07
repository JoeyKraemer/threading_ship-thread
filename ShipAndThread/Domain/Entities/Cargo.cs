using System.ComponentModel.DataAnnotations;
using ShipAndThread.BlackBox;
using ShipAndThread.Domain.Enums;

namespace ShipAndThread.Domain.Entities;

public class Cargo
{
    public required int Id { get; set; }
        
    public int Status { get; set; }
    
    public int TruckId { get; set; }
    public Coordinates Destination { get; set; }
    public Truck? Truck { get; set; }
}