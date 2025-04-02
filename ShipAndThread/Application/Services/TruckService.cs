using ShipAndThread.Domain.Entities;
using ShipAndThread.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
        

namespace ShipAndThread.Application.Services;

public class TruckService
{
    private readonly AppDbContext _context;

    public TruckService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Truck>> GetAllTrucksAsync()
    {
        return await _context.Trucks.Include(t => t.LocationHistory).Include(t => t.CargoList).ToListAsync();
    }

    public async Task<Truck> GetTruckByIdAsync(int id)
    {
        return await _context.Trucks.Include(t => t.LocationHistory).Include(t => t.CargoList).FirstOrDefaultAsync(t => t.TruckId == id);
    }

    public async Task AddTruckAsync(Truck truck)
    {
        _context.Trucks.Add(truck);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTruckAsync(Truck truck)
    {
        _context.Trucks.Update(truck);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTruckAsync(int id)
    {
        var truck = await _context.Trucks.FindAsync(id);
        if (truck != null)
        {
            _context.Trucks.Remove(truck);
            await _context.SaveChangesAsync();
        }
    }
}