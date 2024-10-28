using Microsoft.AspNetCore.SignalR;
using Proo.Service._RideService;

namespace Proo.APIs.Hubs
{
    public class FastRideHub : Hub
    {
        private readonly IRideAcceptanceService _rideAcceptanceService;

        public FastRideHub(IRideAcceptanceService rideAcceptanceService)
        {
            _rideAcceptanceService = rideAcceptanceService;
        }

        public async Task DriverAcceptRide(string driverId)
        {
            _rideAcceptanceService.SetRideAcceptance(driverId, true);
            await Clients.User(driverId).SendAsync("RideAccepted");
        }

        public async Task DriverRejectRide(string driverId)
        {
            _rideAcceptanceService.SetRideAcceptance(driverId, false);
            await Clients.User(driverId).SendAsync("RideRejected");
        }
    }
}
