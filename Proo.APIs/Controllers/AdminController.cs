using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Errors;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;

namespace Proo.APIs.Controllers
{
    
    public class AdminController : BaseApiController
    {
        private readonly ApplicationDbContext _dbContext;

        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        
        [HttpGet("drivers/Pending")]
        public async Task<ActionResult<IEnumerable<Driver>>> GetPendingDrivers()
        {
            var pendingDrivers = await _dbContext.Drivers.Where( e => e.Status == DriverStatus.Pending ).ToListAsync();
            return Ok(pendingDrivers);
        }
        [HttpGet("drivers/approved")]
        public async Task<ActionResult<IEnumerable<Driver>>> GetApprovedDrivers()
        {
            var approvedDrivers = await _dbContext.Drivers.Where( e => e.Status == DriverStatus.Approved).ToListAsync();
            
            return Ok(approvedDrivers);
        }

        [HttpGet("drviers/approve/{id}")]
        public async Task<ActionResult<string>> ApproveDriver(string id)
        {
            var driver = await _dbContext.Drivers.FindAsync( id );
            if ( driver == null )
            {
                return BadRequest(new ApiResponse(400, "The Drive Is Not Exist."));
            }

            driver.Status = DriverStatus.Approved;

            await _dbContext.SaveChangesAsync();
            return Ok("Driver approved successfully.");
        }
        [HttpPost("drivers/reject/{id}")]
        public async Task<IActionResult> RejectDriver(string id)
        {
            var driver = await _dbContext.Drivers.FindAsync(id);
            if (driver == null)
            {
                return BadRequest(new ApiResponse(400, "The Drive Is Not Exist."));
            }


            driver.Status = DriverStatus.Rejected;

            await _dbContext.SaveChangesAsync();

            return Ok("Driver rejected.");
        }

    }
}
