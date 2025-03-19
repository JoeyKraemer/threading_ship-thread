using Microsoft.EntityFrameworkCore;
using ShipAndThread.Domain.Entities;

public class AppDbContext : DbContext
{
    public DbSet<Truck> Trucks { get; set; }
    public DbSet<LocationHistory> LocationHistories { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<CargoList> CargoLists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=app.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CargoList>()
            .HasKey(cl => new { cl.CargoId, cl.TruckId }); // Composite Key

        base.OnModelCreating(modelBuilder);
    }
}