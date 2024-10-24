using Microsoft.AspNetCore.SignalR;
using Proo.APIs.Dtos.Rides;

namespace Proo.APIs.Hubs
{
    public class RideHub : Hub
    {
        public async Task SendRideRequest(RideNotificationDto notification)
        {
            await Clients.Group("NearbyDrivers").SendAsync("ReceiveRideRequest", notification);
            await Clients.Group("NearbyDrivers").SendAsync("ReceiveFastRideRequest", notification);
        }
    }
}
