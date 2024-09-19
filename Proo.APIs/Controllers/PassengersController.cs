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

namespace Proo.APIs.Controllers
{

    public class PassengersController : BaseApiController
    {
        private const string passenger = "passenger";
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PassengersController(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager)
        {

            _userManager = userManager;
        }


        [Authorize(Roles = passenger)]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiToReturnDtoResponse>> GetSpecPassengers()
        {
            var UserPhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone);

            var GetUserByPhone = await _userManager.Users.FirstOrDefaultAsync(U => U.PhoneNumber == UserPhoneNumber);

            if (GetUserByPhone is null) return BadRequest(400);

            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "The Passenger Data",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new ProfileDto
                    {
                        ProfilePictureUrl = GetUserByPhone.ProfilePictureUrl,
                        FullName = GetUserByPhone.FullName,
                        Email = GetUserByPhone.Email,
                        DateOfBirth = (DateTime)GetUserByPhone.DateOfBirth,
                        Gender = GetUserByPhone.Gender,
                        PhoneNumber = GetUserByPhone.PhoneNumber,
                    }

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


            var response = new ApiToReturnDtoResponse
            {
                Data = new DataResponse
                {
                    Mas = "Update Passenger Data Succ",
                    StatusCode = StatusCodes.Status200OK,
                    Body = new ProfileDto
                    {
                        ProfilePictureUrl = GetUserByPhone.ProfilePictureUrl,
                        FullName = GetUserByPhone.FullName,
                        Email = GetUserByPhone.Email,
                        DateOfBirth = (DateTime)GetUserByPhone.DateOfBirth,
                        Gender = GetUserByPhone.Gender,
                        PhoneNumber = GetUserByPhone.PhoneNumber,
                    }

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
        //            Body = new List<object>
        //            {
                        
        //            }
        //        }
        //    };

        //    return Ok(response);
        //}
    }
}
