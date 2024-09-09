using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System.Security.Claims;

namespace Proo.APIs.Controllers
{
    
    public class PassengersController : BaseApiController
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public PassengersController(ApplicationDbContext dbContext,UserManager<ApplicationUser>  userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApplicationUser>> GetSpecPassengers()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            
            return Ok(user);
        }
    }
}
