using Microsoft.EntityFrameworkCore;
using ShipAndThread.Domain.Entities;
using ShipAndThread.Infrastructure.Persistence;

namespace ShipAndThread.Application.Services;

public class LocationHistoryService
{
    private readonly AppDbContext _context;

    public LocationHistoryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<LocationHistory>> GetAllLocationHistoriesAsync()
    {
        return await _context.LocationHistories.Include(lh => lh.Truck).ToListAsync();
    }

    public async Task<LocationHistory> GetLocationHistoryByTruckIdAsync(int truckId)
    {
        return await _context.LocationHistories.Include(lh => lh.Truck).FirstOrDefaultAsync(lh => lh.TruckId == truckId);
    }

    public async Task AddLocationHistoryAsync(LocationHistory locationHistory)
    {
        _context.LocationHistories.Add(locationHistory);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateLocationHistoryAsync(LocationHistory locationHistory)
    {
        _context.LocationHistories.Update(locationHistory);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteLocationHistoryAsync(int truckId)
    {
        var locationHistory = await _context.LocationHistories.FindAsync(truckId);
        if (locationHistory != null)
        {
            _context.LocationHistories.Remove(locationHistory);
            await _context.SaveChangesAsync();
        }
    }
}
