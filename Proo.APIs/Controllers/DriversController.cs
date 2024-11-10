using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Dtos;
using Proo.APIs.Dtos.Driver;
using Proo.APIs.Dtos.Identity;
using Proo.APIs.Dtos.Passenger;
using Proo.APIs.Dtos.RideRequest;
using Proo.APIs.Dtos.Rides;
using Proo.APIs.Errors;
using Proo.APIs.Hubs;
using Proo.Core.Contract;
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Entities;
using Proo.Core.Entities.Driver_Location;
using Proo.Core.Specifications.DriverSpecifiactions;
using Proo.Core.Specifications.PassengerSpecifiactions;
using Proo.Infrastructer.Document;
using Proo.Infrastructer.Repositories;
using Proo.Infrastructer.Repositories.DriverRepository;
using Proo.Service.LocationService;
using StackExchange.Redis;
using System.Security.Claims;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;

namespace Proo.APIs.Controllers
{
    public class DriversController : BaseApiController
    {
        private const string driver = "driver";
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUpdateDriverLocationService _updateLocation;
        private readonly IDriverRepository _driverRepo;
        private readonly IHubContext<LocationHub> _hubContext;

        public DriversController(IUnitOfWork unitOfWork 
            , UserManager<ApplicationUser> userManager
            , IUpdateDriverLocationService updateLocation
            , IDriverRepository driverRepo
            , IHubContext<LocationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _updateLocation = updateLocation;
            _driverRepo = driverRepo;
            _hubContext = hubContext;
        }


        [Authorize(Roles = driver)]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetSpecDriver()
        {
            var driverPhone = User.FindFirstValue(ClaimTypes.MobilePhone);

            var user = await _userManager.Users.FirstOrDefaultAsync(p => p.PhoneNumber == driverPhone);

            if (user is null) return NotFound(new ApiResponse(404, "Not Found Driver."));

            // get spec user 
            var spec = new DriverWithApplicationUserSpecifiaction(user.Id);
            var driver = await  _unitOfWork.Repositoy<Driver>().GetByIdWithSpecAsync(spec);

            if (driver is null) return BadRequest(new ApiResponse(400));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Driver Data",
                    StatusCode = StatusCodes.Status200OK,
                    Body = driver
                    
                }
            };

            return Ok(response);
        }

        [Authorize(Roles = driver)]
        [HttpPut("update_driver")]
        public async Task<ActionResult<ApplicationUser>> UpdateDriverInformation([FromForm] UpdatedDriverDto model)
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);

            if (GetUserByPhone is null) return BadRequest(400);

            GetUserByPhone.FullName = model.FullName;
            GetUserByPhone.Email = model.Email;
            GetUserByPhone.Gender = model.Gender;
            GetUserByPhone.DateOfBirth = model.DataOfBirth;
            GetUserByPhone.ProfilePictureUrl = DocumentSettings.UploadFile(model.UploadFile, "ProfilePicture");

            var driver = await _driverRepo.getByUserId(GetUserByPhone.Id);
            if (driver is null) return BadRequest(new ApiResponse(400));


            driver.DrivingLicenseIdFront = DocumentSettings.UploadFile(model.LicenseIdFront, "LicenseId");
            driver.DrivingLicenseIdBack = DocumentSettings.UploadFile(model.LicenseIdBack, "LicenseId");

            driver.Status = DriverStatus.Avaiable;
            driver.DrivingLicenseExpiringDate = model.ExpiringDate;


            var result = await _userManager.UpdateAsync(GetUserByPhone);
            if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            _unitOfWork.Repositoy<Driver>().Update(driver);
            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0) return BadRequest(new ApiResponse(400));


            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Update user Data Succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>
                    {
                        new DriverToReturnDto
                        {
                            FullName = GetUserByPhone.FullName,
                            //Email = GetUserByPhone.Email,
                            DateOfBirth = (DateTime)GetUserByPhone.DateOfBirth,
                            Gender = GetUserByPhone.Gender,
                            PhoneNumber = GetUserByPhone.PhoneNumber,
                        },
                        driver.DrivingLicenseIdFront,
                       driver.DrivingLicenseIdBack,
                    }
                }
            };

            return Ok(response);

        }


        //[Authorize(Roles = driver)]
        //[HttpPost("update-location")]
        //public async Task<ActionResult<ApiToReturnDtoResponse>> UpdateDriverLocation(DriverLocations driverLocations)
        //{
        //    var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
        //    var user = await  _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        //    if (user is null) return BadRequest(new ApiResponse(401));  // this validation is not important 

        //    var driver = await _driverRepo.getByUserId(user.Id);

        //    if (driver is null) return BadRequest(new ApiResponse(400, "The driver not found"));

        //    if (driverLocations is null /*|| driverLocations.DriverId != driver.Id*/)
        //        return BadRequest(new ApiResponse(400, "Invalid request data."));

        //    if (driverLocations.Latitude < -90 || driverLocations.Latitude > 90 || driverLocations.Longitude < -180 || driverLocations.Longitude > 180)
        //        return BadRequest(new ApiResponse(400 , "Invalid latitude or longitude."));
            
        //    // call Update driver location service 
        //    await _updateLocation.UpdateDriverLocationAsync(driver.Id, driverLocations.Latitude, driverLocations.Longitude , driver.Status , user.Gender);

        //    await _hubContext.Clients.All.SendAsync("ReceiveLocationUpdate", driver.Id, driverLocations.Latitude, driverLocations.Longitude);

        //    return Ok(new ApiToReturnDtoResponse
        //    {
        //        Data = new DataResponse
        //        {
        //            Mas = "Driver location updated successfully.",
        //            StatusCode = StatusCodes.Status200OK ,
        //            Body = driverLocations 
        //        }
        //    });
        //}



        [Authorize(Roles = driver)]
        [HttpPut("Cancel_Ride_By_Driver")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> CancelRideByDriver(CancelRidetDto model)
        {

            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            if (string.IsNullOrEmpty(phoneNumber))
                return Unauthorized(new ApiResponse(401, "Phone number is missing or invalid"));

            var UserByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (UserByPhoneNumber is null)
                return Unauthorized(new ApiResponse(401, "Unauthorized: User not found"));

            var Driver = await _driverRepo.getByUserId(UserByPhoneNumber.Id);
            if (Driver == null) return NotFound(new ApiResponse(404, "Driver Not Found"));

            var Ride = await _unitOfWork.Repositoy<Ride>().GetByIdAsync(model.RideId);
            if (Ride is null)
                return NotFound(new ApiResponse(404, "The Ride is not found."));

            var ride = await _unitOfWork.RideRepository.GetActiveTripForDriver(Driver.Id);
            if (ride == null) return NotFound(new ApiResponse(404, "Not found Ongoing trips"));

            if (ride.Status != RideStatus.CanceledByPassenger
                && ride.Status != RideStatus.Completed
                && ride.Status != RideStatus.WAITING_FOR_PAYMENT) return BadRequest(new ApiResponse(400, "Can Not Canceled Ride "));
        

            ride.Status = RideStatus.CanceledByDriver;
            ride.LastModifiedAt = DateTime.Now;

            _unitOfWork.Repositoy<Ride>().Update(ride);

            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0)
                return BadRequest(new ApiResponse(400, "That error when update entity in database."));

            //Notification This Driver 
            // step 7: Notify the passenger 
            var PassengerId = ride.PassengerId;

            await _hubContext.Clients.User(PassengerId).SendAsync("CanceledRideByDriver", new
            {
                Message = "This Ride Canceled",
            });

            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Ride Canceled and Notification the Passenger",
                    StatusCode = StatusCodes.Status200OK,
                    Body = ride.Id
                }
            });
        }


        [Authorize(Roles = driver)]
        [HttpPost("RatingRideByDriver")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> RatingRideByDriver(RatingDriverDto model)
        {
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            var driver = await _driverRepo.getByUserId(GetUserByPhone.Id);

            if (driver == null) return NotFound(new ApiResponse(404, "driver Not Found"));

            var ride = await _unitOfWork.RideRepository.GetActiveTripForDriver(driver.Id);
            if (ride is null) return NotFound(new ApiResponse(404, "The driver Is Not Going Ride"));

            if (ride.Status != RideStatus.Completed)
                return BadRequest(new ApiResponse(400, "ride Not Completed Yet"));

            if (0 <= model.Score && model.Score <= 5)
                return BadRequest(new ApiResponse(400, "Score Out Of Range"));

            var driverRating = new DriverRating()
            {
                RideId = ride.Id,
                Score = model.Score,
                Review = model.Review,
            };
            _unitOfWork.Repositoy<DriverRating>().Add(driverRating);

            

            var count = await _unitOfWork.CompleteAsync();
            if (count <= 0)
                return BadRequest(new ApiResponse(400, "That error when update entity in database."));

            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Ride Rating By driver ",
                    StatusCode = StatusCodes.Status200OK,
                }
            });
        }

        [Authorize(Roles = driver)]
        [HttpGet("Driver_History_Ride")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetDriverHistoryRide()
        {
            var PhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var UserByPhoneNumber = await _userManager.Users.FirstOrDefaultAsync(_ => _.PhoneNumber == PhoneNumber);

            if (UserByPhoneNumber is null) return NotFound(new ApiResponse(404, "This User Not Found"));

            var driver = await _driverRepo.getByUserId(UserByPhoneNumber.Id);
            if (driver is null) return NotFound(new ApiResponse(404, "This Driver Not Found"));

            var spec = new DriverWithRideAndRatingSpecifications(driver.Id);
            var Rides = await _unitOfWork.Repositoy<Ride>().GetAllWithSpecAsync(spec);

            if (Rides.Count == 0) return NotFound(new ApiResponse(404, "Not Found any Ride For This driver"));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Driver Data",
                    StatusCode = StatusCodes.Status200OK,
                    Body = Rides

                }
            };

            return Ok(response);

        }
    }
}
