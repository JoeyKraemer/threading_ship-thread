using Microsoft.AspNetCore.SignalR;

namespace ShipAndThread.BlackBox;

public class CommunicationHub : Hub
{
    public async Task SendLocation(object locationUpdate)
    {
        await Clients.All.SendAsync("ReceiveLocationUpdate", locationUpdate);
    }
}