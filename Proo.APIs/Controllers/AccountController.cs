using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Dtos.Identity;
using Proo.APIs.Errors;
using Proo.Core.Contract;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Entities;
using System.Security.Claims;

namespace Proo.APIs.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ICachService _cachService;

        public AccountController(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager
            , ITokenService tokenService
            ,ICachService cachService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _cachService = cachService;
        }
        
        [HttpPost("send-otp")]
        public IActionResult SendOtp([FromBody] string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return BadRequest("Phone number is required.");
            }

            // Generate a static OTP
            var otp = "123456";

            // Simulate sending OTP (e.g., logging, printing, etc. - for now we just return it)
            // In real implementation, you would send this OTP to the user's phone number via SMS gateway.
            _cachService.CacheOtpAsync(phoneNumber, otp);

            return Ok(new { PhoneNumber = phoneNumber, Otp = otp });
        }

        public async Task<IActionResult> VerifiOtp([FromBody] string phoneNumber , string otp)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return BadRequest("Phone number is required.");
            }

            var cachedOtp = await _cachService.GetCachedOtpAsync(phoneNumber);
            if (cachedOtp != otp)
            {
                return Ok(new { success = false, Message = "Invalid OTP" });
            }

            return Ok(new { success = true, Message = "Verifi Succsed" });
        }
        // Register endpoint if role is passenger [user]
        [Authorize]
        [HttpPost("userRegister")] // POST : baseurl/api/Account/userRgister
        public async Task<ActionResult<UserDto>> RegisterForUser(RegisterForUserDto model, string role)
        {
            var PhoneForUser = User.FindFirstValue(ClaimTypes.MobilePhone);

            var user = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == PhoneForUser);

            if (user is not null)
            {
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Gender = model.Gender;
                user.UserName = model.Email.Split("@")[0];

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded) return BadRequest(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

                var addRole = await _userManager.AddToRoleAsync(user, role);
                if (!addRole.Succeeded) return BadRequest(new ApiValidationResponse() { Errors = addRole.Errors.Select(E => E.Description) });

                return Ok(new UserDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = _userManager.GetRolesAsync(user).Result,
                    Token = await _tokenService.CreateTokenAsync(user, _userManager)
                });

            }

            return NotFound(new ApiResponse(404, "The mobile phone for user is not existing."));

        }

        //[HttpPost("register")]
        //public async Task<ActionResult<UserLoginDto>> register(LoginDto model)
        //{
        //    var PhoneExisting = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == model.PhoneNumber);

        //    if (PhoneExisting is null) return BadRequest(new ApiResponse(400, "The Phone Is Already Exist."));

        //    var user = new ApplicationUser
        //    {
        //        PhoneNumber = model.PhoneNumber,
        //        Email = "model@gmail.com",
        //        UserName = "model"
        //    };
        //    ///////////

        //    var result = await _userManager.CreateAsync(user);
        //    if (!result.Succeeded) return BadRequest(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

        //    return Ok(new UserLoginDto
        //    {
        //        PhoneNumber = user.PhoneNumber,
        //        Token = await _tokenService.CreateTokenAsync(user, _userManager)
        //    });
        //}
    }
}
