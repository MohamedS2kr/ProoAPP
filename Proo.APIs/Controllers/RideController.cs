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
        private readonly IHubContext<RideHub> _hubContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDriverRepository _driverRepository;
        private const string Driver = "Driver";
        public RideController(IUnitOfWork unitOfWork
            , IMapper mapper,
            IHubContext<RideHub> hubContext,
            UserManager<ApplicationUser> userManager
            , IDriverRepository driverRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
            _userManager = userManager;
            _driverRepository = driverRepository;

        }

        [Authorize(Roles = Driver)]
        [HttpPut("{tripRequestId}/start-trip")]
        public async Task<ActionResult> StartTrip(int tripRequestId)
        {
            // get driver      enhansement
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber ==  phoneNumber);
            if (user is null) return BadRequest(new ApiResponse(401));
            

            // Step 1: check valid ride request exists
            var rideReqeust = await _unitOfWork.Repositoy<RideRequests>().GetByIdAsync(tripRequestId);
            if (rideReqeust is null) return BadRequest(new ApiResponse(400, "Ride Request is not exist."));

            // Step 2: check driver exists
            var driver = await _driverRepository.getByUserId(user.Id);
            if (driver is null) return BadRequest(new ApiResponse(400, "Driver is not exist."));

            // Step 3: check driver has ongoing trips
            var rides = await _unitOfWork.RideRepository.GetActiveTripForDriver(driver.Id);
            if (rides is not null) return BadRequest(new ApiResponse(400, "Driver has ongoing trips"));

            // ** Security check !
            if (rideReqeust.Id != tripRequestId)
                return BadRequest(new ApiResponse(400, "Active trip request for driver does not match !!"));

            // Step 4: prepare entity  TODO

            rideReqeust.Status = RideRequestStatus.TRIP_STARTED;
            // Step 5: create trip entity  TODO using mapper
            var RideEntity = new Ride
            {
                
            };

            // Step 6: perform database operations
            rideReqeust.LastModifiedAt = DateTime.Now;
            _unitOfWork.Repositoy<RideRequests>().Update(rideReqeust);

            _unitOfWork.Repositoy<Ride>().Add(RideEntity);

            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400, "That error when commeted changed in database"));

            return Ok();

        }

        [Authorize(Roles = Driver)]
        [HttpPut("{rideId}/end-ride")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> EndTrip(int rideId)
        {
            // step 1: get user --> driver 
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user is null) return BadRequest(new ApiResponse(401));

            // step 2 : get driver 
            var driver = await _driverRepository.getByUserId(user.Id);
            if (driver is null) return BadRequest(new ApiResponse(400, "The Driver not found"));

            // check driver has ongoing trips 
            var rides = await _unitOfWork.RideRepository.GetActiveTripForDriver(driver.Id);
            if (rides is null) return BadRequest(new ApiResponse(400, "Not found Ongoing trips"));

            if (rideId != rides.Id)
                return BadRequest(new ApiResponse(400, "Rides dosn't match."));

            // check status validations TODO 

            // update sattus for rides
            rides.Status = RideStatus.Completed;
            rides.LastModifiedAt = DateTime.Now;

            _unitOfWork.Repositoy<Ride>().Update(rides);

            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0)
                return BadRequest(new ApiResponse(400, "That error when update entity in database."));

            return Ok();

        }

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
