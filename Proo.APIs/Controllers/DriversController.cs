using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Dtos;
using Proo.APIs.Dtos.Identity;
using Proo.APIs.Errors;
using Proo.Core.Contract;
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Entities;
using Proo.Core.Specifications.DriverSpecifiactions;
using Proo.Infrastructer.Document;
using Proo.Infrastructer.Repositories;
using Proo.Infrastructer.Repositories.DriverRepository;
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
        private readonly IMapper _mapper;
        private readonly IDriverRepository _driverRepo;

        public DriversController(IUnitOfWork unitOfWork 
            , UserManager<ApplicationUser> userManager
            , IMapper mapper
            , IDriverRepository driverRepo)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
            _driverRepo = driverRepo;
        }

        [Authorize(Roles = driver)]
        [HttpPost("register")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> DriverRegister([FromForm]DriverDto model)
        {
            var UserPhone = User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(p => p.PhoneNumber == UserPhone);

            if (user is null) return BadRequest(new ApiResponse(400));



            var driver = new Driver
            {
                UserId = user.Id,
                LicenseIdFront = DocumentSettings.UploadFile(model.LicenseIdFront, "LicenseId"),
                LicenseIdBack = DocumentSettings.UploadFile(model.LicenseIdBack, "LicenseId"),
                ExpiringDate = model.ExpiringDate,
                IsAvailable = model.IsAvailable,
            };
            
            _unitOfWork.Repositoy<Driver>().Add(driver);
            var count = await  _unitOfWork.CompleteAsync();

            if (count <= 0) return BadRequest(new ApiResponse(400 , "The error logged when occured save changed."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The driver register succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>()
                    
                },
                Errors = new List<string>()
            };

            return Ok(response);
        }

        [Authorize(Roles = driver)]
        [HttpGet("getSpecDriver")]
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
                    Mas = "The Passenger Data",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>
                    {
                        driver
                    }
                },
                Errors = new List<string>()
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


            driver.LicenseIdFront = DocumentSettings.UploadFile(model.LicenseIdFront, "LicenseId");
            driver.LicenseIdBack = DocumentSettings.UploadFile(model.LicenseIdBack, "LicenseId");

            driver.IsAvailable = model.IsAvailable;
            driver.ExpiringDate = model.ExpiringDate;


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
                        new UserDto
                        {
                            FullName = GetUserByPhone.FullName,
                            Email = GetUserByPhone.Email,
                            DateOfBirth = (DateTime)GetUserByPhone.DateOfBirth,
                            Gender = GetUserByPhone.Gender,
                            PhoneNumber = GetUserByPhone.PhoneNumber,
                        },
                        driver.LicenseIdFront,
                       driver.LicenseIdBack,
                    }
                },
                Errors = new List<string>()
            };

            return Ok(response);

        }


       

    }
}
