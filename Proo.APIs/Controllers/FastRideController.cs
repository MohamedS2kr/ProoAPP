﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Proo.APIs.Dtos.Rides;
using Proo.APIs.Dtos;
using Proo.APIs.Errors;
using Proo.Core.Entities;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;
using System.Security.Claims;
using AutoMapper;
using Proo.APIs.Hubs;
using Proo.Core.Contract.Passenger_Contract;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Contract;
using Microsoft.EntityFrameworkCore;
using Proo.Service._RideService;
using Proo.Core.Specifications.BidSpecifications;
using Proo.Service.Nearby_Driver_Service;

namespace Proo.APIs.Controllers
{
    
    public class FastRideController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<RideHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPassengerRepository _passengerRepo;
        private readonly INearbyDriverService _nearbyDriversService;
        private const string Passenger = "passenger";
        private const string driver = "driver";
        public FastRideController(IUnitOfWork unitOfWork
                , IMapper mapper
                , IRideService rideService,
                IHubContext<RideHub> hubContext,
                UserManager<ApplicationUser> userManager
                , IPassengerRepository passengerRepo
                , INearbyDriverService nearbyDriversService
            )
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _userManager = userManager;
            _passengerRepo = passengerRepo;
            _nearbyDriversService = nearbyDriversService;
        }
        
        [Authorize(Roles = Passenger)]
        [HttpPost("Request02")]
     
        public async Task<ActionResult<ApiToReturnDtoResponse>> RideRequest(RideRequestDto request)
        {
            // 1- check passenger is exist 
            var userPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == userPhoneNumber);

            var passenger = await _passengerRepo.GetByUserId(user.Id);
            if (passenger is null) return BadRequest(new ApiResponse(400, "The Passenger is not Exist."));


            // 2- check passenger has ongoing trip request 
            var rideRequest = await _unitOfWork.RideRequestRepository.GetActiveTripRequestForPassenger(passenger.Id);
            if (rideRequest is not null) return BadRequest(new ApiResponse(400, "Customer has already a requested trip."));

            // 3- check passenger has ongoing trips

            var unfinishedTrip = await _unitOfWork.RideRepository.GetActiveTripForPassenger(passenger.Id);
            if (unfinishedTrip is not null) return BadRequest(new ApiResponse(400, "Customer has already an ongoing trip."));

            //4- store ride in ride table in database 
            var rideRequestModel = new RideRequests
            {
                PickupAddress = request.PickupAddress,
                PickupLatitude = request.PickupLatitude,
                PickupLongitude = request.PickupLongitude,
                DropoffAddress = request.DropOffAddress,
                DropoffLatitude = request.DropoffLatitude,
                DropoffLongitude = request.DropoffLongitude,
                Category = request.Category,
                CreatedAt = DateTime.Now,
                PassengerId = passenger.Id,
                Status = RideRequestStatus.Requested,
                EstimatedPrice = request.FarePrice, // TODO --> double - decimal 
                //paymentMethod  TODO
            };

            _unitOfWork.Repositoy<RideRequests>().Add(rideRequestModel);


            // 5- find the nearby drivers Ids  and check driver gender 
            if (request.DriverGenderSelection == GenderType.FemaleOnly && user.Gender == Gender.Male)
                return BadRequest(new ApiResponse(400, "This Feature not supported for males"));

            List<RideNotificationDto> rideNotificationDtos = new List<RideNotificationDto>();
            if (request.Category == "FastTripe")
            {
                var nearbyDriverIds = await _nearbyDriversService.GetNearbyAvailableDriversAsync(request.PickupLatitude, request.PickupLongitude, 2, 20, request.DriverGenderSelection.ToString());

                // 6- Notify Drivers using signalR
                foreach (var id in nearbyDriverIds)
                {
                    var notifications = new RideNotificationDto
                    {
                        PickupLat = rideRequestModel.PickupLatitude,
                        PickupLng = rideRequestModel.PickupLongitude,
                        PickupAddress = rideRequestModel.PickupAddress,
                        DropOffLat = rideRequestModel.DropoffLatitude,
                        DropOffLng = rideRequestModel.DropoffLongitude,
                        DropOffAddress = rideRequestModel.DropoffAddress,
                        EstimatedTime = rideRequestModel.EstimatedTime,
                        EstimatedDistance = rideRequestModel.EstimatedDistance,
                        FarePrice = rideRequestModel.EstimatedPrice,
                        PassengerId = rideRequestModel.PassengerId,
                    };
                    rideNotificationDtos.Add(notifications);
                    // send the notification to nearby driver 
                    await _hubContext.Clients.User(id.ToString()).SendAsync("ReceiveFastRideRequest", notifications);
                }
            }
            else
            {
                var nearbyDriverIds = await _nearbyDriversService.GetNearbyAvailableDriversAsync(request.PickupLatitude, request.PickupLongitude, 5, 20, request.DriverGenderSelection.ToString());

                // 6- Notify Drivers using signalR
                foreach (var id in nearbyDriverIds)
                {
                    var notifications = new RideNotificationDto
                    {
                        PickupLat = rideRequestModel.PickupLatitude,
                        PickupLng = rideRequestModel.PickupLongitude,
                        PickupAddress = rideRequestModel.PickupAddress,
                        DropOffLat = rideRequestModel.DropoffLatitude,
                        DropOffLng = rideRequestModel.DropoffLongitude,
                        DropOffAddress = rideRequestModel.DropoffAddress,
                        FarePrice = rideRequestModel.EstimatedPrice,
                        PassengerId = rideRequestModel.PassengerId,
                    };
                    rideNotificationDtos.Add(notifications);
                    // send the notification to nearby driver 
                    await _hubContext.Clients.User(id.ToString()).SendAsync("ReceiveRideRequest", notifications);
                }
            }
            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400));
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Request Data succ and Notifi the drivers",
                    StatusCode = StatusCodes.Status200OK,
                    Body = rideNotificationDtos
                }
            };

            return Ok(response);
        }

        //[Authorize(Roles = driver)]
        //[HttpPost("accept-bid")]
        //public async Task<ActionResult> AcceptBid([FromBody] AcceptBidRequestDto acceptBidDto)
        //{
        //    var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
        //    var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        //    var passenger = await _passengerRepo.GetByUserId(user.Id);
        //    if (passenger is null) return BadRequest(new ApiResponse(400, "Passenger is not exist"));
        //    // step 1 : Get the bid and validate it
        //    var spec = new BidWithRideRequestSpecifications(acceptBidDto.BidId);
        //    var bid = await _unitOfWork.Repositoy<Bid>().GetByIdWithSpecAsync(spec);
        //    if (bid is null) return BadRequest(new ApiResponse(400, "The Bid is not exist."));

        //    // validate bid status 
        //    if (bid.BidStatus != BidStatus.Pending)
        //        return BadRequest(new ApiResponse(400, "The selected bid is not lavailable."));

        //    // trip request is invalid/expired if trip request is older than 1 minute TODO
        //    var onminutesAgo = DateTime.Now.AddMinutes(-1);
        //    if (bid.Ride.LastModifiedAt < onminutesAgo) return BadRequest(new ApiResponse(400, "Ride Reuest is expired."));

        //    // step 2 : Ensure that the driver exists
        //    var driver = await _unitOfWork.Repositoy<Driver>().GetDriverOrPassengerByIdAsync(bid.DriverId);
        //    if (driver is null) return BadRequest(new ApiResponse(400, "The Driver is not exist."));

        //    // Step 3: check driver has ongoing trip requests
        //    var driveronGoingRideReuest = await _unitOfWork.RideRequestRepository.GetActiveTripRequestForDriver(bid.DriverId);
        //    if (driveronGoingRideReuest is not null) return BadRequest(new ApiResponse(400, "Driver has an ongoing Ride request."));

        //    // Step 4: check driver has ongoing trips
        //    var rides = await _unitOfWork.RideRepository.GetActiveTripForDriver(bid.DriverId);
        //    if (rides is not null) return BadRequest(new ApiResponse(400, "Driver has an ongoing trips"));

        //    // check passenger has ongoing ride request 
        //    //var passengerOnGoingRideRequest = await _unitOfWork.RideRequestRepository.GetActiveTripRequestForPassenger(passenger.Id);
        //    //if (passengerOnGoingRideRequest is not null) return BadRequest(new ApiResponse(400, "Passenger has an ongoing Ride request."));

        //    //// check passenger has ongoing trips 
        //    //var rides = await _unitOfWork.RideRepository.GetActiveTripForPassenger(passenger.Id);
        //    //if (rides is not null) return BadRequest(new ApiResponse(400, "Passenger has an ongoing trips."));

        //    // step 5: Validate that the ride request is still open
        //    var rideReuest = bid.Ride;
        //    if (rideReuest is null || rideReuest.Status != RideRequestStatus.Requested)
        //        return BadRequest(new ApiResponse(400, "Ride request is not available."));


        //    // step 6 : Update Ride Request and Bid Status
        //    bid.BidStatus = BidStatus.Accepted;
        //    // bid --> Accepted at 

        //    rideReuest.Status = RideRequestStatus.CUSTOMER_ACCEPTED;
        //    rideReuest.DriverId = bid.DriverId;
        //    rideReuest.LastModifiedAt = DateTime.Now;

        //    _unitOfWork.Repositoy<RideRequests>().Update(rideReuest);


        //    //step 7 : Update other bids to rejected
        //    var bidRepo = _unitOfWork.Repositoy<Bid>();
        //    var BidsSpec = new BidWithOtherBidsForDriverSpecifications(rideReuest.Id, bid.Id);
        //    var otherBids = await bidRepo.GetAllWithSpecAsync(BidsSpec);

        //    foreach (var other in otherBids)
        //    {
        //        other.BidStatus = BidStatus.Rejected;
        //        bidRepo.Update(other);
        //        // notify TODO
        //    }



        //    //step 8 : Update Driver Status to Unavailable
        //    var driverRepo = _unitOfWork.Repositoy<Driver>();
        //    //var driver = await driverRepo.GetDriverOrPassengerByIdAsync(bid.DriverId);
        //    //if (driver is null)
        //    //    return BadRequest(new ApiResponse(400, "Driver is not exist."));

        //    driver.Status = DriverStatus.InRide;
        //    driverRepo.Update(driver);

        //    // Save changes
        //    var count = await _unitOfWork.CompleteAsync();
        //    if (count <= 0) return BadRequest(new ApiResponse(400, "The error when save change in database"));

        //    return Ok(new
        //    {
        //        Message = "Bid accepted successfully.",
        //        BidId = bid.Id,
        //        RideRequestId = rideReuest.Id,
        //        DriverId = bid.DriverId
        //    });
        //}
    }
}
