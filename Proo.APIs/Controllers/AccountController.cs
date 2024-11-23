using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proo.Core.Contract;
using Proo.Core.Contract.Dtos;
using Proo.Core.Contract.Dtos.Identity;
using Proo.Core.Contract.Dtos.Passenger;
using Proo.Core.Contract.Errors;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Entities;
using Proo.Infrastructer.Document;
using System.Drawing;
using System.Security.Claims;
using DataResponse = Proo.Core.Contract.Dtos.ApiToReturnDtoResponse.DataResponse;

namespace Proo.APIs.Controllers
{

    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly IPassengerService _passengerService;
        private readonly IDriverService _driverService;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;

        //private readonly ICachService _cachService;

        public AccountController(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager
            , ITokenService tokenService
            , IUserService userService
            , IPassengerService passengerService
            , IDriverService driverService
            , IUnitOfWork unitOfWork
            , RoleManager<IdentityRole> roleManager
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userService = userService;
            _passengerService = passengerService;
            _driverService = driverService;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
        }


        [HttpPost("ResendOtp")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> ResendOtp(ResendOtpDto model)
        {
            var user = _userService.GetBy(p => p.PhoneNumber == model.PhoneNumber).FirstOrDefault();
            if (user is null) return NotFound(new ApiResponse(404, "The Number Not Registered yet"));

            if (user.OtpExpiryTime < DateTime.Now)
            {
                // generate new otp 
                user.OtpCode = "123456";
                user.OtpExpiryTime = DateTime.Now.AddMinutes(2);
                user.IsPhoneNumberConfirmed = false;

                var result = await _userService.Update(user);
                if (!result.Succeeded)
                    return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

                // send otp by sms provider 
                var response = new ApiToReturnDtoResponse
                {
                    Data = new DataResponse
                    {
                        Mas = "ReSend new Otp succ , check your phone sms ,and verifiy the otp.",
                        StatusCode = StatusCodes.Status200OK,
                        Body = user.OtpCode

                    }

                };

                return Ok(response);
            }


            return BadRequest(new ApiResponse(400, "The Otp is not Expired .. "));
        }


        [HttpPost("VerifyOtp")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> VerifyOtp(VerifiDto model)
        {
            var user = _userService.GetBy(p => p.PhoneNumber == model.PhoneNumber).FirstOrDefault();
            if (user is null) return NotFound(new ApiResponse(404, "The Number Not Registered yet"));

            if (user.OtpCode != model.Otp)
                return BadRequest(new ApiResponse(400, "Invalid OTP."));

            if (user.OtpExpiryTime < DateTime.Now)
                return BadRequest(new ApiResponse(400, "expired OTP."));

            user.IsPhoneNumberConfirmed = true;
            user.IsOtpValid = false;
            //user.OtpCode = null;
            user.OtpExpiryTime = null;

            var result = await _userService.Update(user);
            if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });


            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Verifi Succsed",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new VerifyOtpDto
                    {
                        Token = await _tokenService.CreateTokenAsync(user)
                    }

                }

            });
        }


        // Register endpoint if role is passenger [user]
        [Authorize]
        [HttpPost("Register_for_user")] // POST : baseurl/api/Account/Register_for_user
        public async Task<ActionResult<ApiToReturnDtoResponse>> RegisterForUser(RegisterForUserDto model)
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var GetUserByPhone = _userService.GetBy(U => U.PhoneNumber == UserPhoneNumber).FirstOrDefault();

            if (GetUserByPhone is null) return BadRequest(new ApiResponse(400, "The Number Not Found And Invaild Token Claims"));

            GetUserByPhone.FullName = model.FullName;
            GetUserByPhone.Gender = model.Gender;

            var passenger = new PassengerDto
            {
                UserId = GetUserByPhone.Id
            };



            var addPassenger = _passengerService.Add(passenger);
            if (addPassenger <= 0) return BadRequest(new ApiResponse(400, "The error logged when occured save changed."));

            var result = await _userService.Update(GetUserByPhone);
            if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });


            if (await _userService.ValidateUserRole(GetUserByPhone, "Passenger"))
                return BadRequest(new ApiResponse(400, "The User Is Already assign to this Role"));


            var addRole = await _userService.UpdateUserRole(GetUserByPhone, "Passenger");
            if (!addRole.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = addRole.Errors.Select(E => E.Description) });



            var userDto = new UserDto();

            userDto.FullName = GetUserByPhone.FullName;
            userDto.Gender = GetUserByPhone.Gender;
            userDto.PhoneNumber = GetUserByPhone.PhoneNumber;
            userDto.Role = _userService.GetUserRole(GetUserByPhone).Result;
            userDto.Token = await _tokenService.CreateTokenAsync(GetUserByPhone);


            if (GetUserByPhone.RefreshTokens.Any(t => t.IsActive))
            {
                var ActiveRefreshToken = GetUserByPhone.RefreshTokens.FirstOrDefault(t => t.IsActive);
                userDto.RefreshToken = ActiveRefreshToken.Token;
                userDto.RefreshTokenExpiredation = ActiveRefreshToken.ExpirsesOn;
            }
            else
            {
                var refreshToken = _tokenService.GenerateRefreshtoken();
                userDto.RefreshToken = refreshToken.Token;
                userDto.RefreshTokenExpiredation = refreshToken.ExpirsesOn;
                GetUserByPhone.RefreshTokens.Add(refreshToken);
                var IsSucces = await _userService.Update(GetUserByPhone);
                if (!IsSucces.Succeeded)
                    return Ok(new ApiValidationResponse() { Errors = IsSucces.Errors.Select(E => E.Description) });
            }

            SetRefreshTokenInCookies(userDto.RefreshToken, userDto.RefreshTokenExpiredation);

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Passenger register succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = userDto
                }

            };

            return Ok(response);

        }


        [Authorize]
        [HttpPost("Register_for_driver")] // POST : baseurl/api/Account/Register_for_driver
        public async Task<ActionResult<ApiToReturnDtoResponse>> RegisterFordriver([FromForm] RegisterDriverDto model)
        {

            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var GetUserByPhone = _userService.GetBy(U => U.PhoneNumber == UserPhoneNumber).FirstOrDefault();

            if (GetUserByPhone is null) return BadRequest(new ApiResponse(400, "The Number Not Found And Invaild Token Claims"));

            GetUserByPhone.FullName = model.FullName;
            GetUserByPhone.Gender = model.Gender;
            GetUserByPhone.DateOfBirth = model.DateOfBirth;
            GetUserByPhone.ProfilePictureUrl = DocumentSettings.UploadFile(model.ProfilePictureUrl, "ProfilePicture");

            var result = await _userService.Update(GetUserByPhone);
            if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            var getRole = await _userService.GetUserRole(GetUserByPhone);


            if (await _userService.ValidateUserRole(GetUserByPhone, "Driver"))
                return BadRequest(new ApiResponse(400, "The User Is Already assign to this Role"));


            var addRole = await _userService.UpdateUserRole(GetUserByPhone, "Driver");
            if (!addRole.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = addRole.Errors.Select(E => E.Description) });

            var driver = new DriverDto
            {
                UserId = GetUserByPhone.Id,
                DrivingLicenseIdFront = DocumentSettings.UploadFile(model.LicenseIdFront, "LicenseId"),
                DrivingLicenseIdBack = DocumentSettings.UploadFile(model.LicenseIdBack, "LicenseId"),
                NationalIdFront = DocumentSettings.UploadFile(model.NationalIdFront, "DriverNationalId"),
                NationalIdBack = DocumentSettings.UploadFile(model.NationalIdBack, "DriverNationalId"),
                DrivingLicenseExpiringDate = model.ExpiringDate,
                NationalIdExpiringDate = model.NationalIdExpiringDate
            };

            _driverService.Add(driver);


            Color color = ColorTranslator.FromHtml(model.Colour);

            var vehicle = new Vehicle
            {
                DriverId = driver.Id,
                VehicleLicenseIdFront = DocumentSettings.UploadFile(model.VehicleLicenseIdFront, "VehicleLicenseId"),
                VehicleLicenseIdBack = DocumentSettings.UploadFile(model.VehicleLicenseIdBack, "VehicleLicenseId"),
                VehiclePicture = DocumentSettings.UploadFile(model.VehiclePicture, "VehiclePicture"),
                ExpiringDateOfVehicleLicence = model.VehicleExpiringDate,
                AirConditional = model.AirConditional,
                VehicleModelId = model.VehicleModelId,
                NumberOfPlate = model.NumberOfPalet,
                NumberOfPassenger = model.NumberOfPassenger,
                YeareOfManufacuter = model.YeareOfManufacuter,
                Colour = DocumentSettings.GetColorName(color)
            };

            _unitOfWork.Repositoy<Vehicle>().Add(vehicle);
            var count = await _unitOfWork.CompleteAsync();

            if (count <= 0) return BadRequest(new ApiResponse(400, "The error logged when occured save changed."));

            var driverToReturnDto = new DriverToReturnDto();

            driverToReturnDto.FullName = GetUserByPhone.FullName;
            driverToReturnDto.Gender = GetUserByPhone.Gender;
            driverToReturnDto.PhoneNumber = GetUserByPhone.PhoneNumber;
            driverToReturnDto.DateOfBirth = (DateTime)GetUserByPhone.DateOfBirth;
            driverToReturnDto.Role = _userManager.GetRolesAsync(GetUserByPhone).Result;
            driverToReturnDto.Token = await _tokenService.CreateTokenAsync(GetUserByPhone);
            driverToReturnDto.DrivingLicenseIdFront = driver.DrivingLicenseIdFront;
            driverToReturnDto.DrivingLicenseIdBack = driver.DrivingLicenseIdBack;
            driverToReturnDto.NationalIdFront = driver.NationalIdFront;
            driverToReturnDto.NationalIdBack = driver.NationalIdBack;
            driverToReturnDto.NationalIdExpiringDate = driver.NationalIdExpiringDate;
            driverToReturnDto.ExpiringDateOfDrivingLicense = driver.DrivingLicenseExpiringDate;
            driverToReturnDto.VehicleLicenseIdFront = vehicle.VehicleLicenseIdFront;
            driverToReturnDto.VehicleLicenseIdBack = vehicle.VehicleLicenseIdBack;
            driverToReturnDto.VehicleExpiringDate = vehicle.ExpiringDateOfVehicleLicence;
            driverToReturnDto.VehiclePicture = vehicle.VehiclePicture;
            driverToReturnDto.ColourHexa = model.Colour;
            driverToReturnDto.Colour = vehicle.Colour;
            driverToReturnDto.AirConditional = vehicle.AirConditional;
            driverToReturnDto.VehicleModel = vehicle.vehicleModel?.ModelName ?? "";
            driverToReturnDto.VehicleType = vehicle.vehicleModel?.VehicleType?.TypeName ?? "";
            driverToReturnDto.NumberOfPlate = vehicle.NumberOfPlate;
            driverToReturnDto.NumberOfPassenger = vehicle.NumberOfPassenger;
            driverToReturnDto.VehicleCategory = vehicle.vehicleModel?.VehicleType?.CategoryOfVehicle?.Name ?? "";
            driverToReturnDto.YeareOfManufacuter = vehicle.YeareOfManufacuter;


            if (GetUserByPhone.RefreshTokens.Any(t => t.IsActive))
            {
                var ActiveRefreshToken = GetUserByPhone.RefreshTokens.FirstOrDefault(t => t.IsActive);
                driverToReturnDto.RefreshToken = ActiveRefreshToken.Token;
                driverToReturnDto.RefreshTokenExpiredation = ActiveRefreshToken.ExpirsesOn;
            }
            else
            {
                var refreshToken = _tokenService.GenerateRefreshtoken();
                driverToReturnDto.RefreshToken = refreshToken.Token;
                driverToReturnDto.RefreshTokenExpiredation = refreshToken.ExpirsesOn;
                GetUserByPhone.RefreshTokens.Add(refreshToken);
                var IsSucces = await _userManager.UpdateAsync(GetUserByPhone);
                if (!IsSucces.Succeeded)
                    return Ok(new ApiValidationResponse() { Errors = IsSucces.Errors.Select(E => E.Description) });
            }

            SetRefreshTokenInCookies(driverToReturnDto.RefreshToken, driverToReturnDto.RefreshTokenExpiredation);


            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The driver register succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = driverToReturnDto
                }

            });

        }



        [HttpPost("Login-register")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> Login(LoginDto model)
        {
            var existingUserByPhone = await _userManager.Users.FirstOrDefaultAsync(p => p.PhoneNumber == model.PhoneNumber);

            if (existingUserByPhone is null)
            {
                // register
                var user = new ApplicationUser
                {
                    PhoneNumber = model.PhoneNumber,
                    UserName = model.PhoneNumber,
                    IsPhoneNumberConfirmed = false,
                    IsOtpValid = false,
                    MacAddress = model.MacAddress
                };
                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded) return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

                // Generate OTP ------ pindding

                user.OtpCode = "123456";
                user.IsOtpValid = true;
                user.OtpExpiryTime = DateTime.Now.AddMinutes(2); // Expiry after 2 minutes

                var updateUser = await _userManager.UpdateAsync(user);

                if (!updateUser.Succeeded) return Ok(new ApiValidationResponse() { Errors = updateUser.Errors.Select(E => E.Description) });

                // Send OTP via SMS service [pendding]


                return Ok(new ApiToReturnDtoResponse
                {
                    Data = new DataResponse
                    {
                        Mas = "Registered succ , Verification code sent to your phone.",
                        StatusCode = StatusCodes.Status200OK,

                    }
                });
            }

            // login 

            if (existingUserByPhone.IsOtpValid /*&& existingUserByPhone.OtpExpiryTime < DateTime.Now */) // expired
            {
                existingUserByPhone.OtpExpiryTime = DateTime.Now.AddMinutes(2); // Expiry after 2 minutes

                var updateUser = await _userManager.UpdateAsync(existingUserByPhone);

                if (!updateUser.Succeeded) return Ok(new ApiValidationResponse() { Errors = updateUser.Errors.Select(E => E.Description) });

                // Send OTP via SMS service [pendding]


                return Ok(new ApiToReturnDtoResponse
                {
                    Data = new DataResponse
                    {
                        Mas = "Verification code sent to your phone.",
                        StatusCode = StatusCodes.Status200OK,

                    }
                });
            }

            //if (!existingUserByPhone.IsPhoneNumberConfirmed) return BadRequest(new ApiResponse(400, "Phone number not verified."));

            // cheack mac Address is valid or not 
            if (model.MacAddress.ToUpper() == existingUserByPhone.MacAddress?.ToUpper())
            {
                var loginToreturnDto = new LoginToreturnDto();

                loginToreturnDto.Token = await _tokenService.CreateTokenAsync(existingUserByPhone);
                loginToreturnDto.Otp = existingUserByPhone.OtpCode;
                loginToreturnDto.Roles = _userManager.GetRolesAsync(existingUserByPhone).Result;

                if (existingUserByPhone.RefreshTokens.Any(t => t.IsActive))
                {
                    var ActiveRefreshToken = existingUserByPhone.RefreshTokens.FirstOrDefault(t => t.IsActive);
                    loginToreturnDto.RefreshToken = ActiveRefreshToken.Token;
                    loginToreturnDto.RefreshTokenExpiredation = ActiveRefreshToken.ExpirsesOn;
                }
                else
                {
                    var refreshToken = _tokenService.GenerateRefreshtoken();
                    loginToreturnDto.RefreshToken = refreshToken.Token;
                    loginToreturnDto.RefreshTokenExpiredation = refreshToken.ExpirsesOn;
                    existingUserByPhone.RefreshTokens.Add(refreshToken);
                    var IsSucces = await _userManager.UpdateAsync(existingUserByPhone);
                    if (!IsSucces.Succeeded)
                        return Ok(new ApiValidationResponse() { Errors = IsSucces.Errors.Select(E => E.Description) });
                }

                if (!string.IsNullOrEmpty(loginToreturnDto.RefreshToken))
                    SetRefreshTokenInCookies(loginToreturnDto.RefreshToken, loginToreturnDto.RefreshTokenExpiredation);

                return Ok(new ApiToReturnDtoResponse
                {
                    Data = new DataResponse
                    {
                        Mas = "Logined succ.",
                        StatusCode = StatusCodes.Status200OK,
                        Body = loginToreturnDto
                    }
                });

            }
            // Generate OTP

            existingUserByPhone.OtpCode = "123456";
            existingUserByPhone.IsOtpValid = true;
            existingUserByPhone.OtpExpiryTime = DateTime.Now.AddMinutes(1); // Expiry after 2 minutes
            existingUserByPhone.IsPhoneNumberConfirmed = false;
            existingUserByPhone.MacAddress = model.MacAddress;
            var updated = await _userManager.UpdateAsync(existingUserByPhone);

            if (!updated.Succeeded) return Ok(new ApiValidationResponse() { Errors = updated.Errors.Select(E => E.Description) });

            // Send OTP via SMS service


            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Verification code sent to your phone..",
                    StatusCode = StatusCodes.Status200OK,

                }
            });

        }




        [HttpGet("refreshToken")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> RefreshToken()
        {
            var token = Request.Cookies["refreshToken"];

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user is null) return BadRequest(new ApiResponse(400, "Invalid Token"));

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive) return BadRequest(new ApiResponse(400, "InActive Token"));

            refreshToken.RevokedOn = DateTime.Now;

            var newrefreshToekn = _tokenService.GenerateRefreshtoken();
            user.RefreshTokens.Add(newrefreshToekn);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            var userDto = new UserDto();

            userDto.Token = await _tokenService.CreateTokenAsync(user);
            userDto.FullName = user.FullName;
            userDto.Gender = user.Gender;
            userDto.PhoneNumber = user.PhoneNumber;
            userDto.Role = await _userManager.GetRolesAsync(user);
            userDto.RefreshToken = newrefreshToekn.Token;
            userDto.RefreshTokenExpiredation = newrefreshToekn.ExpirsesOn;

            SetRefreshTokenInCookies(userDto.RefreshToken, userDto.RefreshTokenExpiredation);

            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "refresh token succ.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = userDto
                }
            });
        }

        [HttpPost("revokedToken")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> RevokedToken(RevokedTokenDto modelDto)
        {
            var token = modelDto.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token)) return BadRequest(new ApiResponse(400, "token is required"));

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user is null) return BadRequest(new ApiResponse(400, "Invalid Token"));

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive) return BadRequest(new ApiResponse(400, "InActive Token"));

            refreshToken.RevokedOn = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            return Ok(new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Revoked Token succ.",
                    StatusCode = StatusCodes.Status200OK,

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


                }
            });
        }



        private void SetRefreshTokenInCookies(string refreshToken, DateTime Expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = Expires.ToLocalTime(),
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }



    }
}
