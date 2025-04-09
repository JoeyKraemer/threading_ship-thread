using Microsoft.AspNetCore.SignalR;
using ShipAndThread.Infrastructure.Persistence;

namespace ShipAndThread.BlackBox;

public interface ITruckDataSimulationService
{
    Task StartSimulationAsync();
}

public class TruckDataSimulationService : ITruckDataSimulationService
{
    private readonly IServiceProvider _services;

    public TruckDataSimulationService(IServiceProvider services)
    {
        _services = services;
    }

    public async Task StartSimulationAsync()
    {
        using var scope = _services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<CommunicationHub>>();
        await AsyncDataGeneration.Go(context, hubContext);
    }
}