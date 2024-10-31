using Microsoft.AspNetCore.SignalR;
using Proo.APIs.Dtos.Rides;
using Proo.Core.Contract;
using Proo.Core.Entities;
using Proo.Service.Nearby_Driver_Service;

namespace Proo.APIs.Hubs
{
    public class RideRequestHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INearbyDriverService _nearbyDriverService;

        public RideRequestHub(IUnitOfWork unitOfWork , INearbyDriverService nearbyDriverService)
        {
            _unitOfWork = unitOfWork;
            _nearbyDriverService = nearbyDriverService;
        }

        public async Task NotifyNearbyDrivers(int rideRequestId , GenderType genderType)
        {
            // Retrieve ride request details from database
            var rideRequest = await _unitOfWork.Repositoy<RideRequests>().GetByIdAsync(rideRequestId);
            if (rideRequest == null) throw new HubException("Ride request not found.");

            // Find passenger's details
            var passenger = await _unitOfWork.Repositoy<Passenger>().GetDriverOrPassengerByIdAsync(rideRequest.PassengerId);
            if (passenger is null) throw new HubException("passenger is not found");

            var user = passenger.User;

            if (genderType == GenderType.FemaleOnly && user.Gender == Gender.Male)
                  throw new HubException("This Feature not supported for males");

                // Get nearby drivers based on criteria
                var nearbyDriverIds = await _nearbyDriverService.GetNearbyAvailableDriversAsync(
                rideRequest.PickupLatitude, rideRequest.PickupLongitude, 5, 20, genderType.ToString()
            );

            // Prepare and send notifications to each driver
            foreach (var driverId in nearbyDriverIds)
            {
                var notification = new RideNotificationDto
                {
                    PickupLat = rideRequest.PickupLatitude,
                    PickupLng = rideRequest.PickupLongitude,
                    PickupAddress = rideRequest.PickupAddress,
                    DropOffLat = rideRequest.DropoffLatitude,
                    DropOffLng = rideRequest.DropoffLongitude,
                    DropOffAddress = rideRequest.DropoffAddress,
                    FarePrice = rideRequest.EstimatedPrice,
                    PassengerId = rideRequest.PassengerId,
                    Picture = user.ProfilePictureUrl ?? "",
                    Name = user.FullName ?? "",
                    NumberOfTrips = await _unitOfWork.RideRepository.GetRidesCountForPassenger(rideRequest.PassengerId)
                };

                // Send the notification to the specific driver
                await Clients.User(driverId.ToString()).SendAsync("ReceiveRideRequest", notification);
            }
        }
    }
}
