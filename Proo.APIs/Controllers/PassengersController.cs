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

        public PassengersController(UserManager<ApplicationUser> userManager
                                    ,SignInManager<ApplicationUser> signInManager
                                    ,IMapper mapper
                                    ,IUnitOfWork unitOfWork,
                                     IRideRequestRepository rideRequestRepo)
        {

            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _rideRequestRepo = rideRequestRepo;
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
                    //Body = new ProfileDto
                    //{
                    //    ProfilePictureUrl = GetUserByPhone.ProfilePictureUrl,
                    //    FullName = GetUserByPhone.FullName,
                    //    Email = GetUserByPhone.Email,
                    //    DateOfBirth = (DateTime)GetUserByPhone.DateOfBirth,
                    //    Gender = GetUserByPhone.Gender,
                    //    PhoneNumber = GetUserByPhone.PhoneNumber,

                    //}
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
                    //Body = new ProfileDto
                    //{
                    //    ProfilePictureUrl = GetUserByPhone.ProfilePictureUrl,
                    //    FullName = GetUserByPhone.FullName,
                    //    Email = GetUserByPhone.Email,
                    //    DateOfBirth = (DateTime)GetUserByPhone.DateOfBirth,
                    //    Gender = GetUserByPhone.Gender,
                    //    PhoneNumber = GetUserByPhone.PhoneNumber,
                    //}
                    Body = mappedResult

                }
            };

            return Ok(response);

        }



        [Authorize(Roles = passenger)]
        [HttpPost("Cancel_Trip_Requset")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> CancelTripRequest()
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var passenger = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);
            
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
                    Mas = "Cancel TripRequest Successed",
                    StatusCode = StatusCodes.Status200OK,
                    Body=""
                }
            };

            return Ok(response);

        }
        [Authorize(Roles = passenger)]
        [HttpPost("Reject_Offer_By_Passenger")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> RejectOfferByPassenger()
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var passenger = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);
            
            if (passenger is null)
                return NotFound(new ApiResponse(404, "The Passenger Not Found"));
            
            var requestedTrip = await _rideRequestRepo.GetActiveTripRequestForPassenger(passenger.Id);
            
            if (requestedTrip is null)
                return BadRequest(new ApiResponse(400, "Customer has no pending requested trip."));
            
            
            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Cancel TripRequest Successed",
                    StatusCode = StatusCodes.Status200OK,
                    Body=""
                }
            };

            return Ok(response);

        }

        //[Authorize(Roles = passenger)]
        //[HttpPost("FindDriver")]
        //public async Task<ActionResult<ApplicationUser>> FindDriver(FindDriverDto model)
        //{
        //    var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

        //    var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);

        //    if (GetUserByPhone is null) return BadRequest(400);


        //    //احسب المسافة بين النقطتين واحسب الاجره و اخزن ده و ابداء اعرضه علي السواقين
        //    // محتاج كمان ارجع المسافة و الوقت والسعر المنطقي للرحله و كمان اسم المشتخدم 
        //    // passenger واخزن ده في الداتا بيز و ابداء في جزء بقي اني انا سواق و بدور علي 

        //    var response = new ApiToReturnDtoResponse
        //    {
        //        Data = new DataResponse
        //        {
        //            Mas = "The Passenger Data",
        //            StatusCode = StatusCodes.Status200OK,
        //            Body = 
        //        }
        //    };

        //    return Ok(response);
        //}
    }
}
