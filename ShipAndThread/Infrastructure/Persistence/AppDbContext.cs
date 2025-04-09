using Microsoft.EntityFrameworkCore;
using ShipAndThread.Domain.Entities;

namespace ShipAndThread.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Truck> Trucks { get; set; }
    public DbSet<Cargo> Cargoes { get; set; }
    public DbSet<LocationHistory> LocationHistories { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=:memory:;Cache=Shared;Mode=ReadWrite");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure Truck primary key
        modelBuilder.Entity<Truck>()
            .HasKey(t => t.TruckId);

        // Configure Truck -> LocationHistory (1-to-1 relationship)
        modelBuilder.Entity<Truck>()
            .HasOne(t => t.LocationHistory)
            .WithOne(lh => lh.Truck)
            .HasForeignKey<LocationHistory>(lh => lh.TruckId) // LocationHistory owns the foreign key
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade); // Cascade delete LocationHistory when Truck is deleted

        // Configure Truck -> Cargo (1-to-many relationship)
        modelBuilder.Entity<Truck>()
            .HasMany(t => t.CargoList)
            .WithOne(c => c.Truck)
            .HasForeignKey(c => c.TruckId)
            .OnDelete(DeleteBehavior.SetNull); // Don't cascade delete Cargo when Truck is deleted
    }
}