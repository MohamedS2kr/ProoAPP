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
using Proo.Core.Entities;
using Proo.Infrastructer.Document;
using StackExchange.Redis;
using System.Security.Claims;
using static Proo.APIs.Dtos.ApiToReturnDtoResponse;

namespace Proo.APIs.Controllers
{
    public class DriversController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DriversController(IUnitOfWork unitOfWork 
            , UserManager<ApplicationUser> userManager
            , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        [Authorize(Roles ="driver")]
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

        //[Authorize(Roles ="driver")]
        //[HttpGet("getSpecDriver")]
        //public async Task<ActionResult> GetSpecDriver(string driverId)
        //{

        //}
    }
}
