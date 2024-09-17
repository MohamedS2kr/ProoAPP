using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Dtos;
using Proo.APIs.Dtos.Identity;
using Proo.APIs.Errors;
using Proo.Core.Contract;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Entities;
using Proo.Infrastructer.Data;
using Proo.Infrastructer.Document;
using System.Drawing;
using System.Security.Claims;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;

namespace Proo.APIs.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        //private readonly ICachService _cachService;

        public AccountController(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager
            , ITokenService tokenService
            , IUnitOfWork unitOfWork
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }


        [HttpPost("VerifyOtp")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> VerifyOtp(VerifiDto model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(p => p.PhoneNumber == model.PhoneNumber);
            if (user is null) return BadRequest(new ApiResponse(400));

            if (user.OtpCode != model.Otp || user.OtpExpiryTime < DateTime.UtcNow)
                return BadRequest(new ApiResponse(400, "Invalid or expired OTP."));

            user.IsPhoneNumberConfirmed = true;
            user.OtpCode = null;
            user.OtpExpiryTime = null;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) 
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Verifi Succsed",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>()
                    {
                        new VerifyOtpDto
                        {
                            Token = await _tokenService.CreateTokenAsync(user , _userManager)
                        }
                    }
                    
                }
             
            };

            return Ok(response);
        }


        // Register endpoint if role is passenger [user]
        [Authorize]
        [HttpPost("Register_for_user")] // POST : baseurl/api/Account/Register_for_user
        public async Task<ActionResult<ApiToReturnDtoResponse>> RegisterForUser(RegisterForUserDto model )
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);

            if (GetUserByPhone is null) return BadRequest(400);

            GetUserByPhone.FullName = model.FullName;
            GetUserByPhone.Gender = model.Gender;


            var result = await _userManager.UpdateAsync(GetUserByPhone);
            if (!result.Succeeded)
            return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            var getRole = await _userManager.GetRolesAsync(GetUserByPhone);

            if (getRole is null || getRole[0] != model.Role)
            {
                var addRole = await _userManager.AddToRoleAsync(GetUserByPhone, model.Role);
                if (!addRole.Succeeded)
                    return Ok(new ApiValidationResponse() { Errors = addRole.Errors.Select(E => E.Description) });
            }

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The User register succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>
                    {
                        new UserDto
                        {
                            FullName = GetUserByPhone.FullName,
                            Gender = GetUserByPhone.Gender,
                            PhoneNumber = GetUserByPhone.PhoneNumber,
                            Role = _userManager.GetRolesAsync(GetUserByPhone).Result,
                            Token = await _tokenService.CreateTokenAsync(GetUserByPhone , _userManager)
                        }
                    }
                }
               
            };

            return Ok(response);
        }


        [Authorize]
        [HttpPost("Register_for_driver")] // POST : baseurl/api/Account/Register_for_driver
        public async Task<ActionResult<ApiToReturnDtoResponse>> RegisterFordriver([FromForm] DriverDto model)
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);

            if (GetUserByPhone is null) return BadRequest(400);

            GetUserByPhone.FullName = model.FullName;
            GetUserByPhone.Gender = model.Gender;
            GetUserByPhone.DateOfBirth = model.DateOfBirth;
            

            var result = await _userManager.UpdateAsync(GetUserByPhone);
            if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            var getRole = await _userManager.GetRolesAsync(GetUserByPhone);

            if(getRole is null || getRole[0] != model.Role)
            {
                var addRole = await _userManager.AddToRoleAsync(GetUserByPhone, model.Role);
                if (!addRole.Succeeded)
                    return Ok(new ApiValidationResponse() { Errors = addRole.Errors.Select(E => E.Description) });
            }

            var driver = new Driver
            {
                UserId = GetUserByPhone.Id,
                LicenseIdFront = DocumentSettings.UploadFile(model.LicenseIdFront, "LicenseId"),
                LicenseIdBack = DocumentSettings.UploadFile(model.LicenseIdBack, "LicenseId"),
                ExpiringDate = model.ExpiringDate,
                IsAvailable = model.IsAvailable,
            };

            _unitOfWork.Repositoy<Driver>().Add(driver);
            

            Color color = ColorTranslator.FromHtml(model.Colour);

            var vehicle = new Vehicle
            {
                DriverId = driver.Id,
                VehicleLicenseIdFront = DocumentSettings.UploadFile(model.VehicleLicenseIdFront, "VehicleLicenseId"),
                VehicleLicenseIdBack = DocumentSettings.UploadFile(model.VehicleLicenseIdBack, "VehicleLicenseId"),
                ExpiringDate = model.VehicleExpiringDate,
                AirConditional = model.AirConditional,
                category = model.category,
                NumberOfPalet = model.NumberOfPalet,
                NumberOfPassenger = model.NumberOfPassenger,
                Type = model.Type,
                YeareOfManufacuter = model.YeareOfManufacuter,
                Colour = DocumentSettings.ColorToHex(color)
            };

            _unitOfWork.Repositoy<Vehicle>().Add(vehicle);
            var count = await _unitOfWork.CompleteAsync();

            if (count <= 0) return BadRequest(new ApiResponse(400, "The error logged when occured save changed."));

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The driver register succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>
                    {
                        new DriverToReturnDto
                        {
                            FullName = GetUserByPhone.FullName,
                            Gender = GetUserByPhone.Gender,
                            PhoneNumber = GetUserByPhone.PhoneNumber,
                            DateOfBirth = (DateTime) GetUserByPhone.DateOfBirth,
                            Role = _userManager.GetRolesAsync(GetUserByPhone).Result,
                            Token = await _tokenService.CreateTokenAsync(GetUserByPhone , _userManager),
                            LicenseIdFront = driver.LicenseIdFront,
                            LicenseIdBack = driver.LicenseIdBack,
                            ExpiringDate = driver.ExpiringDate,
                            IsAvailable = driver.IsAvailable,
                            VehicleLicenseIdFront = vehicle.VehicleLicenseIdFront,
                            VehicleLicenseIdBack = vehicle.VehicleLicenseIdBack,
                            VehicleExpiringDate = vehicle.ExpiringDate,
                            Colour = vehicle.Colour,
                            AirConditional = vehicle.AirConditional,
                            category = vehicle.category,
                            NumberOfPalet = vehicle.NumberOfPalet,
                            NumberOfPassenger = vehicle.NumberOfPassenger,
                            Type = vehicle.Type,
                            YeareOfManufacuter = vehicle.YeareOfManufacuter
                        }
                    }
                }
               
            };

            return Ok(response);
        }


        [HttpPost("Login")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> Login(LoginDto model)
        {
            var existingUserByPhone = await _userManager.Users.FirstOrDefaultAsync(p => p.PhoneNumber == model.PhoneNumber);

            if(existingUserByPhone is null)
            {
                // register
                var user = new ApplicationUser
                {
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.PhoneNumber,
                    IsPhoneNumberConfirmed = false
                };
                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded) return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

                // Generate OTP

                user.OtpCode = "123456";
                user.OtpExpiryTime = DateTime.UtcNow.AddDays(5); // Expiry after 5 days

                var updateUser = await _userManager.UpdateAsync(user);

                if (!updateUser.Succeeded) return Ok(new ApiValidationResponse() { Errors = updateUser.Errors.Select(E => E.Description) });

                // Send OTP via SMS service [pendding]

              
                return Ok(new ApiToReturnDtoResponse
                {
                    Data = new DataResponse
                    {
                        Mas = "Registered succ , Verification code sent to your phone.",
                        StatusCode = StatusCodes.Status200OK,
                        Body = new List<object>()

                    }
                });
            }

            // login 
            if (!existingUserByPhone.IsPhoneNumberConfirmed) return BadRequest(new ApiResponse(400, "Phone number not verified."));


            // Generate OTP

            existingUserByPhone.OtpCode = "123456";
            existingUserByPhone.OtpExpiryTime = DateTime.UtcNow.AddDays(5); // Expiry after 5 days

            var updated = await _userManager.UpdateAsync(existingUserByPhone);

            if (!updated.Succeeded) return Ok(new ApiValidationResponse() { Errors = updated.Errors.Select(E => E.Description) });

            // Send OTP via SMS service


            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Logined succ , Verification code sent to your phone.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>()

                }
            });

        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Logged out successfully",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>()

                }
            });
        }


    
    }
}
