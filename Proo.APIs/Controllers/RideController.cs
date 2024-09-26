using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Dtos;
using Proo.APIs.Dtos.Rides;
using Proo.APIs.Errors;
using Proo.APIs.Hubs;
using Proo.Core.Contract;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using Proo.Service._RideService;
using System.Security.Claims;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;

namespace Proo.APIs.Controllers
{

    public class RideController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRideService _rideService;
        private readonly IHubContext<RideHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private const string Passenger = "passenger";
        private const string Driver = "Driver";
        public RideController(IUnitOfWork unitOfWork
            , IMapper mapper
            , IRideService rideService,
            IHubContext<RideHub> hubContext,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rideService = rideService;
            _hubContext = hubContext;
            _userManager = userManager;
        }


        //[HttpPost("request")]
        //public async Task<ActionResult<ApiToReturnDtoResponse>> RideRequest(RideRequestDto request)
        //{
        //    // store ride in ride table in database 
        //    var RideMapped = _mapper.Map<Ride>(request);
        //    RideMapped.PassengerId = request.PassengerId;
        //    RideMapped.Status = RideStatus.Requested;
        //    RideMapped.CreatedAt = DateTime.UtcNow;

        //    _unitOfWork.Repositoy<Ride>().Add(RideMapped);
        //    var count = await _unitOfWork.CompleteAsync();
        //    if (count <= 0) return BadRequest(new ApiResponse(400));

        //    // find the nearby drivers 
        //    var nearbyDrivers = await _rideService.GetNearbyDrivers(request.PickupLat, request.PickupLng, 5);

        //    // Notify Drivers using signalR
        //    foreach (var driver in nearbyDrivers)
        //    {
        //        var notifications = new RideNotificationDto
        //        {
        //            PickupLat = RideMapped.PickupLocation.Latitude,
        //            PickupLng = RideMapped.PickupLocation.Longitude,
        //            PickupAddress = RideMapped.PickupLocation.Address,
        //            DropOffLat = RideMapped.DestinationLocation.Latitude,
        //            DropOffLng = RideMapped.DestinationLocation.Longitude,
        //            DropOffAddress = RideMapped.DestinationLocation.Address,
        //            FarePrice = RideMapped.FarePrice,
        //            PassengerId = RideMapped.PassengerId,

        //        };

        //        // send the notification to nearby driver 
        //        await _hubContext.Clients.User(driver.Id).SendAsync("ReceiveRideRequest", notifications);
        //    }

        //    var response = new ApiToReturnDtoResponse
        //    {
        //        Data = new DataResponse
        //        {
        //            Mas = "The Request Data succ",
        //            StatusCode = StatusCodes.Status200OK,
        //            Body = new List<object>
        //            {
        //                _mapper.Map<RdieToReturnDto>(RideMapped)
        //            }
        //        }
        //    };

        //    return Ok(response);
        //}

        [Authorize(Roles = Passenger)]
        [HttpPost("request")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> RideRequest(RideRequestDto request)
        {
            // 1- check passenger is exist 
            var userPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == userPhoneNumber);

            var passenger = await _passengerRepo.getByUserId(user.Id);
            if (passenger is null) return BadRequest(new ApiResponse(400, "The Passenger is not Exist."));


            // 2- check passenger has ongoing trip request 
            var rideRequest = _unitOfWork.rideRequestRepository.GetActiveTripRequestForCustomer(passenger.Id);
            if (rideRequest is not null) return BadRequest(new ApiResponse(400, "Customer has already a requested trip."));

            // 3- check passenger has ongoing trips

            var unfinishedTrip = await _unitOfWork.RideRepository.GetActiveTripForCustomer(passenger.Id);
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
            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400));

            // 5- find the nearby drivers   TODO
            var nearbyDrivers = await _rideService.GetNearbyDrivers(request.PickupLatitude, request.PickupLongitude, 5);

            // 6- Notify Drivers using signalR
            foreach (var driver in nearbyDrivers)
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

                // send the notification to nearby driver 
                await _hubContext.Clients.User(driver.Id).SendAsync("ReceiveRideRequest", notifications);
            }

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Request Data succ and Notifi the drivers",
                    StatusCode = StatusCodes.Status200OK,
                    Body = rideRequestModel
                }
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitBid(BidDto bidDto)
        {
            // Step 1: check valid trip request exists
            var rideRequest = await _unitOfWork.Repositoy<RideRequests>().GetByIdAsync(bidDto.RideRequestsId);
            if (rideRequest is null) return BadRequest(new ApiResponse(400, "The Ride Reqeust is not found."));

            // trip request is invalid/expired if trip request is older than 1 minute TODO
            var onminutesAgo = DateTime.Now.AddMinutes(-1);
            if (rideRequest.LastModifiedAt < onminutesAgo) return BadRequest(new ApiResponse(400, "Ride Reuest is expired."));

            // Step 2: check driver exists
            var driver = await _unitOfWork.Repositoy<Driver>().GetDriverOrPassengerByIdAsync(bidDto.DriverId);
            if (driver is null) return BadRequest(new ApiResponse(400, "Driver is not found"));

            // Step 3: check driver has ongoing trip requests
            var ongoingRideRequest = await _unitOfWork.rideRequestRepository.GetActiveTripRequestForDriver(bidDto.DriverId);
            if (ongoingRideRequest is not null) return BadRequest(new ApiResponse(400, "Driver has an ongoing Ride request."));

            // Step 4: check driver has ongoing trips
            //var Ride = _unitOfWork.RideRepository.get

            // step 5: Get cat info by driver 
            var vehcile =
            // Step 6: create Bid entity


        }


        //[Authorize(Roles = Passenger)]
        //[HttpPost("CreateRideRequest_FindDriver")]
        //public async Task<ActionResult<ApiToReturnDtoResponse>> CreateRideRequest(RideRequestDto dto)
        //{
        //    var UserPhone = User.FindFirstValue(ClaimTypes.MobilePhone);
        //    var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhone);
        //    if (GetUserByPhone is null) return BadRequest(new ApiResponse(400, "The Number Not Found And Invaild Token Claims"));

        //    var result = new LocationService().CalculateDestanceAndTimeAndPrice(dto.PickupLatitude, dto.PickupLongitude, dto.DropoffLatitude, dto.DropoffLongitude , dto.Category);

        //    var RideRequest = new RideRequests()
        //    {
        //        PassengerId = GetUserByPhone.Id,
        //        DropoffAddress = dto.DropOffAddress,
        //        DropoffLatitude = dto.DropoffLatitude,
        //        DropoffLongitude = dto.DropoffLongitude,

        //        PickupAddress = dto.PickupAddress,
        //        PickupLatitude = dto.PickupLatitude,
        //        PickupLongitude = dto.PickupLongitude,

        //        Category = dto.Category,

        //        EstimatedDistance = result.distance,
        //        EstimatedTime = result.estimatedTime,
        //        EstimatedPrice = result.price,

        //        Status = Status.Pending,
        //        CreatedAt = DateTime.Now,
        //    };
        //    _unitOfWork.Repositoy<RideRequests>().Add(RideRequest);
        //    var count = await _unitOfWork.CompleteAsync();
        //    if (count <= 0) return BadRequest(new ApiResponse(400));


        //    //find the nearby drivers 
        //    var nearbyDrivers = await _rideService.GetNearbyDrivers(dto.PickupLatitude, dto.PickupLongitude, 5);

        //    ///    // Notify Drivers using signalR
        //    ///    foreach (var driver in nearbyDrivers)
        //    ///    {
        //    ///        var notifications = new RideNotificationDto
        //    ///        {
        //    ///            PickupLat = RideMapped.PickupLocation.Latitude,
        //    ///            PickupLng = RideMapped.PickupLocation.Longitude,
        //    ///            PickupAddress = RideMapped.PickupLocation.Address,
        //    ///            DropOffLat = RideMapped.DestinationLocation.Latitude,
        //    ///            DropOffLng = RideMapped.DestinationLocation.Longitude,
        //    ///            DropOffAddress = RideMapped.DestinationLocation.Address,
        //    ///            FarePrice = RideMapped.FarePrice,
        //    ///            PassengerId = RideMapped.PassengerId,
        //    ///
        //    ///        };
        //    ///
        //    ///        // send the notification to nearby driver 
        //    ///        await _hubContext.Clients.User(driver.Id).SendAsync("ReceiveRideRequest", notifications);
        //    ///    }


        //    var response = new ApiToReturnDtoResponse
        //    {
        //        Data = new DataResponse
        //        {
        //            Mas = "The Request Data succ",
        //            StatusCode = StatusCodes.Status200OK,
        //            Body = new ReturnRideRequestDto
        //            {
        //                ProfilePicture = GetUserByPhone.ProfilePictureUrl,
        //                Name = GetUserByPhone.UserName,
        //                PhoneNumber = GetUserByPhone.PhoneNumber,
        //                Category = dto.Category,
        //                PickupAddress = RideRequest.PickupAddress,
        //                DropOffAddress = RideRequest.DropoffAddress,
        //                Price = RideRequest.EstimatedPrice,
        //                Time = RideRequest.EstimatedTime,
        //                Distance = RideRequest.EstimatedDistance,
        //            }
        //        }
        //    };

        //    return Ok(response);
        //}

        


        [HttpPost("calc_price_time_destance")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> CalcPriceAndTimeAndDestance(calculatePriceAnddectaceDto CalcDto)
        {
            var result = new LocationService().CalculateDestanceAndTimeAndPrice(CalcDto.PickUpLat, CalcDto.PickUpLon, CalcDto.DroppOffLat, CalcDto.DroppOffLon,CalcDto.Category);

            ///if (CalcDto.Category == "Ride")
            ///{
            ///    return (new ApiToReturnDtoResponse
            ///    {
            ///        Data = new DataResponse
            ///        {
            ///            Mas = "The Price and Time Calculated by Distance and Category [Ride]",
            ///            StatusCode = StatusCodes.Status200OK,
            ///            Body = new ReturnCalcDto()
            ///            {
            ///                Price = result.price,
            ///                Distance = result.distance,
            ///                Time = result.estimatedTime
            ///            }
            ///        }
            ///    });
            ///}
            ///else if (CalcDto.Category == "Comfort")
            ///{
            ///    return (new ApiToReturnDtoResponse
            ///    {
            ///        Data = new DataResponse
            ///        {
            ///            Mas = "The Price and Time Calculated by Distance and Category [Comfort]",
            ///            StatusCode = StatusCodes.Status200OK,
            ///            Body = new ReturnCalcDto()
            ///            {
            ///                Price = result.price + 50,
            ///                Distance = result.distance,
            ///                Time = result.estimatedTime
            ///            }
            ///        }
            ///    });
            ///}
            ///else if (CalcDto.Category == "Scoter")
            ///{
            ///    return (new ApiToReturnDtoResponse
            ///    {
            ///        Data = new DataResponse
            ///        {
            ///            Mas = "The Price and Time Calculated by Distance and Category [Scoter]",
            ///            StatusCode = StatusCodes.Status200OK,
            ///            Body = new ReturnCalcDto()
            ///            {
            ///                Price = result.price - 50,
            ///                Distance = result.distance,
            ///                Time = result.estimatedTime
            ///            }
            ///        }
            ///    });
            ///}

            return (new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = $"The Price and Time Calculated by Distance and Category [{CalcDto.Category}]",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new ReturnCalcDto()
                    {
                        Price = result.price,
                        Distance = result.distance,
                        Time = result.estimatedTime
                    }
                }
            });
        }


    }
}
