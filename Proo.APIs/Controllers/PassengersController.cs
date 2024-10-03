using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Dtos.Identity;
using Proo.APIs.Dtos;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System.Security.Claims;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;
using Proo.APIs.Errors;
using Proo.Infrastructer.Document;
using Proo.APIs.Dtos.Passenger;
using AutoMapper;
using Proo.Core.Contract;
using Proo.Infrastructer.Repositories.DriverRepository;
using Proo.Core.Contract.RideService_Contract;
using Proo.Infrastructer.Data;
using Proo.Core.Contract.Passenger_Contract;
using Proo.APIs.Dtos.Rides;
using Proo.Core.Specifications.BidSpecifications;
using System.Security.Cryptography;
using Microsoft.AspNetCore.SignalR;
using Proo.APIs.Hubs;

namespace Proo.APIs.Controllers
{

    public class PassengersController : BaseApiController
    {
        private const string passenger = "passenger";
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRideRequestRepository _rideRequestRepo;
        private readonly IPassengerRepository _passengerRepos;
        private readonly IHubContext<RideHub> _hubContext;

        public PassengersController(UserManager<ApplicationUser> userManager
                                    ,SignInManager<ApplicationUser> signInManager
                                    ,IMapper mapper
                                    ,IUnitOfWork unitOfWork,
                                     IRideRequestRepository rideRequestRepo,
                                     IPassengerRepository passengerRepos,
                                     IHubContext<RideHub> hubContext)
        {

            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _rideRequestRepo = rideRequestRepo;
            _passengerRepos = passengerRepos;
            _hubContext = hubContext;
        }


        [Authorize(Roles = passenger)]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetSpecPassengers()
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);

            if (GetUserByPhone is null) return BadRequest(400);
            var result = _mapper.Map<ProfileDto>(GetUserByPhone);
            
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Passenger Data",
                    StatusCode = StatusCodes.Status200OK,
                    Body = result

                }
            };

            return Ok(response);

        }
        
        [Authorize(Roles = passenger)]
        [HttpPost("Update_for_Passenger")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> UpdateSpecPassengers([FromForm] updateUserDto model)
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);

            if (GetUserByPhone is null) return BadRequest(400);

            GetUserByPhone.FullName = model.FullName;
            GetUserByPhone.Email = model.Email;
            GetUserByPhone.Gender = model.Gender;
            GetUserByPhone.DateOfBirth = model.DataOfBirth;
            GetUserByPhone.ProfilePictureUrl = DocumentSettings.UploadFile(model.UploadFile, "ProfilePicture");


            var result = await _userManager.UpdateAsync(GetUserByPhone);
            if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            var mappedResult = _mapper.Map<ProfileDto>(GetUserByPhone);
            
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Update Passenger Data Succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = mappedResult

                }
            };

            return Ok(response);

        }



        [Authorize(Roles = passenger)]
        [HttpPost("Cancel_Trip_Request")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> CancelTripRequest()
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);
            var passenger =await _passengerRepos.GetByUserId(user.Id);
            if (passenger is null)
                return NotFound(new ApiResponse(404, "The Passenger Not Found"));
            
            var requestedTrip = await _rideRequestRepo.GetActiveTripRequestForPassenger(passenger.Id);
            if (requestedTrip is null)
                return BadRequest(new ApiResponse(400, "Customer has no pending requested trip."));
            
            if(requestedTrip.Status == RideRequestStatus.TRIP_STARTED) 
                return NotFound(new ApiResponse(404, "Cannot cancel a trip that has already started."));

            requestedTrip.Status = RideRequestStatus.CUSTOMER_CANCELED;
            requestedTrip.LastModifiedAt = DateTime.Now;

            _unitOfWork.Repositoy<RideRequests>().Update(requestedTrip);

            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400));
            
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Cancel TripRequest Succussed",
                    StatusCode = StatusCodes.Status200OK,
                    
                }
            };

            return Ok(response);

        }
        [Authorize(Roles = passenger)]
        [HttpPut("Reject_Offer_By_Passenger")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> RejectOfferByPassenger(RejectBidRequestDto rejectBidDto)
        {
            //1. Get User and Check is Exist or not 
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);
            
            var passenger = await _passengerRepos.GetByUserId(user.Id);
            if (passenger is null)
                return NotFound(new ApiResponse(404, "The Passenger Not Found"));

            //2. Check passenger has ongoing Ride Request 
            var requestedTrip = await _rideRequestRepo.GetActiveTripRequestForPassenger(passenger.Id);
            if (requestedTrip is null)
                return BadRequest(new ApiResponse(400, "Customer has no pending requested trip."));

            //3. Get Bid
            var spec = new BidWithRideRequestSpecifications(rejectBidDto.BidId);
            var bid = await _unitOfWork.Repositoy<Bid>().GetByIdWithSpecAsync(spec);
            if (bid is null)
                return BadRequest(new ApiResponse(400, "The Select Bid Is Not Found "));

            // trip request is invalid/expired if trip request is older than 1 minute TODO
            var onminutesAgo = DateTime.Now.AddMinutes(-1);
            if (bid.Ride.LastModifiedAt < onminutesAgo) return BadRequest(new ApiResponse(400, "Ride Reuest is expired."));

            //Check BidStatus
            if (bid.BidStatus == BidStatus.Rejected)
                return BadRequest(new ApiResponse(400, "This Bid Is Rejected"));
            
            // Update
            bid.BidStatus = BidStatus.Rejected;

            _unitOfWork.Repositoy<Bid>().Update(bid);
            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400, "The error when save change in database"));

            //Notification This Driver 
            // step 7: Notify the passenger 
            var DriverId = bid.DriverId;
            
            await _hubContext.Clients.User(DriverId).SendAsync("RejectedBid", new
            {    
                Message = "This Offer Rejected",
            });

            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Bid Rejected and Notification the Driver",
                    StatusCode = StatusCodes.Status200OK,
                }
            });

        }

        [Authorize(Roles = passenger)]
        [HttpPut("Cancel_Ride_By_Passenger")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> CancelRideByPassenger()
        {

            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            if (string.IsNullOrEmpty(phoneNumber))
                return Unauthorized(new ApiResponse(401, "Phone number is missing or invalid"));

            var UserByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (UserByPhoneNumber is null)
                return Unauthorized(new ApiResponse(401, "Unauthorized: User not found"));

            var Passenger = await _passengerRepos.GetByUserId(UserByPhoneNumber.Id);
            if (Passenger == null) return NotFound(new ApiResponse(404, "Passenger Not Found"));

            var ride = await _unitOfWork.RideRepository.GetActiveTripForPassenger(Passenger.Id);
            if (ride == null) return NotFound(new ApiResponse(404, "Not found Ongoing trips"));

            if (ride.Status != RideStatus.CanceledByDriver 
                && ride.Status != RideStatus.Completed
                && ride.Status != RideStatus.WAITING_FOR_PAYMENT) return BadRequest(new ApiResponse(400, "Can Not Canceled Ride "));


            ride.Status = RideStatus.CanceledByPassenger;
            ride.LastModifiedAt = DateTime.Now;

            _unitOfWork.Repositoy<Ride>().Update(ride);

            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0)
                return BadRequest(new ApiResponse(400, "That error when update entity in database."));

            //Notification This Driver 
            // step 7: Notify the passenger 
            var DriverId = ride.DriverId;

            await _hubContext.Clients.User(DriverId).SendAsync("CanceledRideByPassenger", new
            {
                Message = "This Ride Canceled",
            });

            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Ride Canceled and Notification the Driver",
                    StatusCode = StatusCodes.Status200OK,
                }
            });
        }

    }
}
