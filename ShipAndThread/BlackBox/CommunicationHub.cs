using Microsoft.AspNetCore.SignalR;
using ShipAndThread.Domain.Entities;

namespace ShipAndThread.BlackBox;

public class CommunicationHub : Hub
{
    public async Task SendLocation(object locationUpdate)
    {
        await Clients.All.SendAsync("ReceiveLocationUpdate", locationUpdate);
    }
    
    public async Task SendCargoUpdate(List<Cargo> updatedCargoList)
    {
        await Clients.All.SendAsync("ReceiveCargoUpdate", updatedCargoList);
    }

}