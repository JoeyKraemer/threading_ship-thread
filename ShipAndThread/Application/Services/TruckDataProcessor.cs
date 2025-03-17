using Microsoft.EntityFrameworkCore;
using ShipAndThread.Application.DTOs;
using ShipAndThread.Domain.Entities;
using ShipAndThread.Domain.Enums;
using ShipAndThread.Infrastructure.Data;

public class TruckDataProcessor
{
    private readonly LogisticsDbContext _dbContext;

    public TruckDataProcessor(LogisticsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ProcessTruckUpdateAsync(TruckUpdateDto dto)
    {
        var truck = await GetOrCreateTruckAsync(dto);
        await UpdateTruckLocationAsync(truck, dto);
        await RecordLocationUpdateAsync(truck, dto);
        await _dbContext.SaveChangesAsync();
    }
    
    private async Task<Truck> GetOrCreateTruckAsync(TruckUpdateDto dto)
    {
        var truck = await _dbContext.Trucks
            .Include(t => t.Driver)
            .FirstOrDefaultAsync(t => t.LicensePlate == dto.LicensePlate);

        if (truck == null)
        {
            truck = new Truck
            {
                LicensePlate = dto.LicensePlate,
                Status = TruckStatus.Idle,
                CurrentLocation = new Location()
            };
            await _dbContext.Trucks.AddAsync(truck);
        }
        return truck;
    }

    private async Task UpdateTruckLocationAsync(Truck truck, TruckUpdateDto dto)
    {
        truck.CurrentLocation = new Location
        {
            X = dto.X,
            Y = dto.Y,
            Z = dto.Z,
            Status = dto.Status
        };

        truck.Status = dto.Status;

        _dbContext.Trucks.Update(truck);
    }


    private async Task RecordLocationUpdateAsync(Truck truck, TruckUpdateDto dto)
    {
        var update = new LocationUpdate
        {
            TruckId = truck.TruckId,
            Timestamp = DateTime.UtcNow,
            Latitude = dto.X, // If X is Latitude
            Longitude = dto.Y, // If Y is Longitude
        };
        await _dbContext.LocationUpdates.AddAsync(update);
    }
}