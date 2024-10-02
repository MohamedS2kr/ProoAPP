using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Dtos;
using Proo.APIs.Dtos.Driver;
using Proo.APIs.Dtos.Identity;
using Proo.APIs.Errors;
using Proo.Core.Contract;
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Entities;
using Proo.Core.Entities.Driver_Location;
using Proo.Core.Specifications.DriverSpecifiactions;
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

        public DriversController(IUnitOfWork unitOfWork 
            , UserManager<ApplicationUser> userManager
            , IUpdateDriverLocationService updateLocation
            , IDriverRepository driverRepo)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _updateLocation = updateLocation;
            _driverRepo = driverRepo;
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
                    Body = new List<object>
                    {
                        driver
                    }
                }
            };

            return Ok(response);
        }

        [Authorize(Roles = driver)]
        [HttpPut("update_drvier")]
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


        [Authorize(Roles = driver)]
        [HttpPost("update-location")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> UpdateDriverLocation(DriverLocations driverLocations)
        {
            var phoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await  _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            if (user is null) return BadRequest(new ApiResponse(401));  // this validation is not important 

            var driver = await _driverRepo.getByUserId(user.Id);

            if (driver is null) return BadRequest(new ApiResponse(400, "The driver not found"));

            if (driverLocations is null || driverLocations.DriverId != driver.Id)
                return BadRequest(new ApiResponse(400, "Invalid request data."));

            if (driverLocations.Latitude < -90 || driverLocations.Latitude > 90 || driverLocations.Longitude < -180 || driverLocations.Longitude > 180)
                return BadRequest(new ApiResponse(400 , "Invalid latitude or longitude."));
            
            // call Update driver location service 
            await _updateLocation.UpdateDriverLocationAsync(driverLocations.DriverId, driverLocations.Latitude, driverLocations.Longitude , driver.Status);

            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Driver location updated successfully.",
                    StatusCode = StatusCodes.Status200OK ,
                    Body = driverLocations 
                }
            });
        }

    }
}
