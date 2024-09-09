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
using Proo.Infrastructer.Document;
using System.Security.Claims;

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
           // ,ICachService cachService
           ,IUnitOfWork unitOfWork
           )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;

            // _cachService = cachService;
        }
       // Dictionary<string, string> SavedOTP = new Dictionary<string, string>();
       
        [HttpPost("send-otp")]
        public IActionResult SendOtp(SendOTPDto model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                return BadRequest("Phone number is required.");
            }

            // Generate a static OTP
            var otp = "123456";

            // Simulate sending OTP (e.g., logging, printing, etc. - for now we just return it)
            // In real implementation, you would send this OTP to the user's phone number via SMS gateway.
            // _cachService.CacheOtpAsync(model.PhoneNumber, otp);
           
          //  SavedOTP.Add(model.PhoneNumber, otp);
            return Ok(new { Phone = model.PhoneNumber, Otp = otp });
        }
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp(VerifiDto model)
        {
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                return BadRequest("Phone number is required.");
            }

            //  var cachedOtp = await _cachService.GetCachedOtpAsync(model.PhoneNumber);
            var cachedOtp = "123456";//SavedOTP[model.PhoneNumber];
            if (cachedOtp != model.Otp)
            {
                return Ok(new { success = false, Message = "Invalid OTP" });
            }

            return Ok(new { success = true, Message = "Verifi Succsed" });
        }
        // Register endpoint if role is passenger [user]
        
        [HttpPost("Register")] // POST : baseurl/api/Account/Rgister
        public async Task<ActionResult<UserDto>> RegisterForUser(RegisterForUserDto model)
        {
            var PhoneExisting = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == model.PhoneNumber);

            if (PhoneExisting !=null) 
                
                return Ok(new ApiResponse(200, "The Phone Is Already Exist."));

            //var user = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == PhoneForUser);

            var user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.PhoneNumber,
                Gender = model.Gender,
                DateOfBirth = model.DataOfBirth,
            };
                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

                var addRole = await _userManager.AddToRoleAsync(user, model.Role);
                if (!addRole.Succeeded) 
                return Ok(new ApiValidationResponse() { Errors = addRole.Errors.Select(E => E.Description) });

                return Ok(new UserDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = _userManager.GetRolesAsync(user).Result,
                    Token = await _tokenService.CreateTokenAsync(user, _userManager)
                });
        }

        //End poit register for driver
        [HttpPost("driverRegister")]
        public async Task<ActionResult<DriverRegisterDto>> RegisterForDriver(DriverDto model, string role)
        {
            var user = new ApplicationUser()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user);
            if(!result.Succeeded) return Ok(new ApiValidationResponse() { Errors = result.Errors.Select(E => E.Description) });

            var drvierData = new Driver()
            {
                UserId = user.Id,
                LicenseIdFront = DocumentSettings.UploadFile(model.LicenseIdFront , "LicenseId"),
                LicenseIdBack = DocumentSettings.UploadFile(model.LicenseIdBack, "LicenseId"),
                ExpiringDate = model.ExpiringDate,
                Status = DriverStatus.Pending
            };

            var driverRepo = _unitOfWork.Repositoy<Driver>();
            driverRepo.Add(drvierData);

            var VehicleData = new Vehicle
            {
                DriverId = drvierData.Id,
                Colour = model.Colour,
                AirConditional = model.AirConditional,
                category = model.category,
                Type = model.Type,
                NumberOfPassenger = model.NumberOfPassenger,
                NumberOfPalet = model.NumberOfPalet,
                ExpiringDate = model.ExpiringDate,
                YeareOfManufacuter = model.YeareOfManufacuter,
                VehicleLicenseIdFront = DocumentSettings.UploadFile(model.VehicleLicenseIdFront, "VehicleLicenseId"),
                VehicleLicenseIdBack = DocumentSettings.UploadFile(model.VehicleLicenseIdBack, "VehicleLicenseId")
            };

            var VehicleRepo = _unitOfWork.Repositoy<Vehicle>();
            VehicleRepo.Add(VehicleData);

            var count = await _unitOfWork.CompleteAsync(); // save cahnge
            if (count <= 0) return BadRequest(new ApiResponse(400));

            return Ok(new DriverRegisterDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = await  _tokenService.CreateTokenAsync(user, _userManager)
            });

        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserLoginDto>> Login(LoginDto model)
        {
            var PhoneExisting = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == model.PhoneNumber);

            if (PhoneExisting is null) return BadRequest(new ApiResponse(400, "The Phone Is Already Exist."));

            return Ok(new { success = true, Message = "Login Succsed"});
        }
    }
}
