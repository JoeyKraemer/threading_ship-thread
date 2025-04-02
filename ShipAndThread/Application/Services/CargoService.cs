using ShipAndThread.Domain.Entities;
using ShipAndThread.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace ShipAndThread.Application.Services;

public class CargoService
{
    private readonly AppDbContext _context;

    public CargoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Cargo>> GetAllCargoAsync()
    {
        return await _context.Cargoes.Include(c => c.Truck).ToListAsync();
    }

    public async Task<Cargo> GetCargoByIdAsync(int id)
    {
        return await _context.Cargoes.Include(c => c.Truck).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task AddCargoAsync(Cargo cargo)
    {
        _context.Cargoes.Add(cargo);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCargoAsync(Cargo cargo)
    {
        _context.Cargoes.Update(cargo);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteCargoAsync(int id)
    {
        var cargo = await _context.Cargoes.FindAsync(id);
        if (cargo != null)
        {
            _context.Cargoes.Remove(cargo);
            await _context.SaveChangesAsync();
        }
    }
}