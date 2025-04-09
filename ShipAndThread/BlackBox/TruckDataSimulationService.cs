using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ShipAndThread.Infrastructure.Persistence;
using ShipAndThread.Application.Services;

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
        var truckService = scope.ServiceProvider.GetRequiredService<TruckService>();
        var cargoService = scope.ServiceProvider.GetRequiredService<CargoService>();
        var locationHistoryService = scope.ServiceProvider.GetRequiredService<LocationHistoryService>();
        
        await AsyncDataGeneration.Go(context, hubContext, truckService, cargoService, locationHistoryService);
    }
}