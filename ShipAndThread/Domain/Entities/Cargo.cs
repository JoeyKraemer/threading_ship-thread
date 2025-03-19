using System.ComponentModel.DataAnnotations;
using ShipAndThread.Domain.Enums;

namespace ShipAndThread.Domain.Entities;

public class Cargo
{
    [Key]
    public int Id { get; set; }
        
    public CargoStatus Status { get; set; }
        
    public double DestinationLongitude { get; set; }
    public double DestinationLatitude { get; set; }
        
    public ICollection<CargoList> CargoLists { get; set; }
}