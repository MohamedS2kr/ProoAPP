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

        public async Task JoinDriverGroup(string driverId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, driverId);
        }

        // Remove driver from group when disconnecting
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, Context.UserIdentifier);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
