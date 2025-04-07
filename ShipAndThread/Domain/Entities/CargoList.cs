using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ShipAndThread.Domain.Entities;

public class CargoList
{
    public required int CargoId { get; set; }
    public required int TruckId { get; set; }
        
    [ForeignKey("CargoId")]
    public required Cargo Cargo { get; set; }
        
    [ForeignKey("TruckId")]
    public required Truck Truck { get; set; }
}