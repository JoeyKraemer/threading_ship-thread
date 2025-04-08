using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Application.Services;

public class TestFixture
{
    public IServiceProvider ServiceProvider { get; }

    public TestFixture()
    {
        var services = new ServiceCollection();

        // Configure in-memory SQLite and services
        var sqliteConnection = new SqliteConnection("Data Source=:memory:;Cache=Shared;Mode=ReadWrite");
        sqliteConnection.Open();

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(sqliteConnection));

        services.AddScoped<LocationHistoryService>();

        ServiceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureCreated();
    }
}