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
using Proo.Infrastructer.Document;
using System.Security.Claims;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;

namespace Proo.APIs.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;

        //private readonly ICachService _cachService;

        public AccountController(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager
            , ITokenService tokenService
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
           
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
                    
                },
                Errors = new List<string>()
            };

            return Ok(response);
        }


        // Register endpoint if role is passenger [user]
        [Authorize]
        [HttpPost("Register_for_user")] // POST : baseurl/api/Account/Register_for_user
        public async Task<ActionResult<ApiToReturnDtoResponse>> RegisterForUser([FromForm]RegisterForUserDto model )
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

            var addRole = await _userManager.AddToRoleAsync(GetUserByPhone, model.Role);
            if (!addRole.Succeeded) 
            return Ok(new ApiValidationResponse() { Errors = addRole.Errors.Select(E => E.Description) });

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
                            Email = GetUserByPhone.Email,
                            DateOfBirth = (DateTime)GetUserByPhone.DateOfBirth,
                            Gender = GetUserByPhone.Gender,
                            PhoneNumber = GetUserByPhone.PhoneNumber,
                            Role = _userManager.GetRolesAsync(GetUserByPhone).Result,
                            Token = await _tokenService.CreateTokenAsync(GetUserByPhone , _userManager)
                        }
                    }
                },
                Errors = new List<string>()
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
                        Mas = "Verification code sent to your phone.",
                        StatusCode = StatusCodes.Status200OK,
                        Body = new List<object>()

                    },
                    Errors = new List<string>()
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
                    Mas = "Verification code sent to your phone.",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new List<object>()

                },
                Errors = new List<string>()
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

                },
                Errors = new List<string>()
            });
        }


    
    }
}
