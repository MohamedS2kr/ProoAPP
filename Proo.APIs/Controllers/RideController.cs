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
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Contract.Passenger_Contract;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Core.Specifications.BidSpecifications;
using Proo.Core.Specifications.DriverSpecifiactions;
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
        private readonly IPassengerRepository _passengerRepo;
        private readonly IDriverRepository _driverRepo;
        private const string Passenger = "passenger";
        private const string Driver = "Driver";
        public RideController(IUnitOfWork unitOfWork
            , IMapper mapper
            , IRideService rideService,
            IHubContext<RideHub> hubContext,
            UserManager<ApplicationUser> userManager
            , IPassengerRepository passengerRepo)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _rideService = rideService;
            _hubContext = hubContext;
            _userManager = userManager;
            _passengerRepo = passengerRepo;

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

            var passenger = await _passengerRepo.GetByUserId(user.Id);
            if (passenger is null) return BadRequest(new ApiResponse(400, "The Passenger is not Exist."));


            // 2- check passenger has ongoing trip request 
            var rideRequest = _unitOfWork.RideRequestRepository.GetActiveTripRequestForPassenger(passenger.Id);
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
               // EstimatedPrice = request.FarePrice, // TODO --> double - decimal 
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
                   // FarePrice = rideRequestModel.EstimatedPrice,
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

        [HttpPost("SubmitBid")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> SubmitBid(BidDto bidDto)
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
            var ongoingRideRequest = await _unitOfWork.RideRequestRepository.GetActiveTripRequestForDriver(bidDto.DriverId);
            if (ongoingRideRequest is not null) return BadRequest(new ApiResponse(400, "Driver has an ongoing Ride request."));

            // Step 4: check driver has ongoing trips
            var rides = await _unitOfWork.RideRepository.GetActiveTripForDriver(bidDto.DriverId);
            if (rides is not null) return BadRequest(new ApiResponse(400, "Driver has ongoing Trip."));


            // step 5: Get car info by driver 
            var spec = new DriverWithVehicleSpecifications(bidDto.DriverId);
            var vehcile = await _unitOfWork.Repositoy<Vehicle>().GetByIdWithSpecAsync(spec);

            // Step 6: create Bid entity
            var bid = _mapper.Map<Bid>(bidDto);
            bid.CreatedAt = DateTime.Now;
            bid.BidStatus = BidStatus.Pendding;

            _unitOfWork.Repositoy<Bid>().Add(bid);
            var addBid = await _unitOfWork.CompleteAsync();
            if (addBid <= 0) return BadRequest(new ApiResponse(400, "The error when save changes in Database"));

            // step 7: Notify the passenger 
            var passengerId = rideRequest.PassengerId;
            await _hubContext.Clients.User(passengerId).SendAsync("ReceiveBid", new
            {
                DriverId = bid.DriverId,
                DriverName = driver.User.FullName,
                DriverProfilePicture = driver.User.ProfilePictureUrl,
                ProposedPrice = bid.OfferedPrice,
                EstimatedArrivalTime = bid.Eta,
                //VehicleType = vehcile.Type,
                //VehicleCategory = vehcile.category

            });

            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Bid succ and Notifi the Passenger",
                    StatusCode = StatusCodes.Status200OK,
                    
                }
            });  // TODO

        }

        [Authorize(Roles =Passenger)]
        [HttpPost("accept-bid")]
        public async Task<ActionResult> AcceptBid([FromBody]AcceptBidRequestDto acceptBidDto)
        {
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            var passenger = await _passengerRepo.GetByUserId(user.Id);
            if (passenger is null) return BadRequest(new ApiResponse(400, "Passenger is not exist"));
            // step 1 : Get the bid and validate it
            var spec = new BidWithRideRequestSpecifications(acceptBidDto.BidId);
            var bid = await _unitOfWork.Repositoy<Bid>().GetByIdWithSpecAsync(spec);
            if (bid is null) return BadRequest(new ApiResponse(400, "The Bid is not exist."));

            // validate bid status 
            if (bid.BidStatus != BidStatus.Pendding)
                return BadRequest(new ApiResponse(400, "The selected bid is not lavailable."));

            // trip request is invalid/expired if trip request is older than 1 minute TODO
            var onminutesAgo = DateTime.Now.AddMinutes(-1);
            if (bid.Ride.LastModifiedAt < onminutesAgo) return BadRequest(new ApiResponse(400, "Ride Reuest is expired."));

            //// step 2 : Ensure that the driver exists
            //var driver = await _unitOfWork.Repositoy<Driver>().GetDriverOrPassengerByIdAsync(DriverId);
            //if (driver is null) return BadRequest(new ApiResponse(400, "The Driver is not exist."));

            //// Step 3: check driver has ongoing trip requests
            //var driveronGoingRideReuest = await _unitOfWork.RideRequestRepository.GetActiveTripRequestForDriver(DriverId);
            //if (driveronGoingRideReuest is not null) return BadRequest(new ApiResponse(400, "Driver has an ongoing Ride request."));

            //// Step 4: check driver has ongoing trips
            //var rides = await _unitOfWork.RideRepository.GetActiveTripForDriver(DriverId);
            //if (rides is not null) return BadRequest(new ApiResponse(400, "Driver has an ongoing trips"));

            // check passenger has ongoing ride request 
            var passengerOnGoingRideRequest = await _unitOfWork.RideRequestRepository.GetActiveTripRequestForPassenger(passenger.Id);
            if(passengerOnGoingRideRequest is not null) return BadRequest(new ApiResponse(400, "Passenger has an ongoing Ride request."));

            // check passenger has ongoing trips 
            var rides = await _unitOfWork.RideRepository.GetActiveTripForPassenger(passenger.Id);
            if (rides is not null) return BadRequest(new ApiResponse(400, "Passenger has an ongoing trips."));

            // step 5: Validate that the ride request is still open
            var rideReuest = bid.Ride;
            if (rideReuest is null || rideReuest.Status != RideRequestStatus.Requested)
                return BadRequest(new ApiResponse(400, "Ride request is not available."));


            // step 6 : Update Ride Request and Bid Status
            bid.BidStatus = BidStatus.Accepted;
            // bid --> Accepted at 

            rideReuest.Status = RideRequestStatus.CUSTOMER_ACCEPTED;
            rideReuest.DriverId = bid.DriverId;

            _unitOfWork.Repositoy<RideRequests>().Update(rideReuest);


            //step 7 : Update other bids to rejected
            var bidRepo = _unitOfWork.Repositoy<Bid>();
            var BidsSpec = new BidWithOtherBidsForDriverSpecifications(rideReuest.Id, bid.Id);
            var otherBids = await bidRepo.GetAllWithSpecAsync(BidsSpec);

            foreach (var other in otherBids)
            {
                other.BidStatus = BidStatus.Rejected;
                bidRepo.Update(other);
            }

            

            //step 8 : Update Driver Status to Unavailable
            var driverRepo = _unitOfWork.Repositoy<Driver>();
            var driver = await driverRepo.GetDriverOrPassengerByIdAsync(bid.DriverId);
            if (driver is null)
                return BadRequest(new ApiResponse(400, "Driver is not exist."));

            driver.Status = DriverStatus.InRide;
            driverRepo.Update(driver);

            // Save changes
            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400, "The error when save change in database"));

            return Ok(new
            {
                Message = "Bid accepted successfully.",
                BidId = bid.Id,
                RideRequestId = rideReuest.Id,
                DriverId = bid.DriverId
            });
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
