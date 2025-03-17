using ShipAndThread.Domain.Entities;

namespace ShipAndThread.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;

public class LogisticsDbContext : DbContext
{
    public LogisticsDbContext(DbContextOptions<LogisticsDbContext> options) : base(options) { }

    public DbSet<Truck> Trucks { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<LocationUpdate> LocationUpdates { get; set; }
    public DbSet<Event> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Map Location as owned type
        modelBuilder.Entity<Truck>().OwnsOne(t => t.CurrentLocation);
    }
}

