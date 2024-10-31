using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proo.APIs.Errors;
using Proo.Core.Contract;
using Proo.Core.Entities;
using Proo.Core.Entities.Driver_Location;
using Proo.Infrastructer.Repositories.DriverRepository;
using Proo.Service.LocationService;
using System.Security.Claims;

namespace Proo.APIs.Hubs
{
    public class LocationHub : Hub
    {
        private readonly IUpdateDriverLocationService _updateLocation;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DriverRepository _driverRepo;

        public LocationHub(IUpdateDriverLocationService updateLocation 
            , IUnitOfWork unitOfWork
            , UserManager<ApplicationUser> userManager
            , DriverRepository driverRepo)
        {
            _updateLocation = updateLocation;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _driverRepo = driverRepo;
        }
        public async Task UpdateLocation(DriverLocations driverLocations)
        {
            var phoneNumber = Context.User.FindFirstValue(ClaimTypes.MobilePhone);
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

            if (user is null)
            {
                throw new Exception("User not found");
            }

            var driver = await _driverRepo.getByUserId(user.Id);

            if (driver == null)
            {
                throw new Exception("The driver not found");
            }
            var vehicles = driver.Vehicles;

            var vehicleType = vehicles.Select(v => v.vehicleModel?.VehicleType?.TypeName).FirstOrDefault();

            if (driverLocations is null /*|| driverLocations.DriverId != driver.Id*/)
                throw new HubException("Invalid request data.");
            if (driverLocations.Latitude < -90 || driverLocations.Latitude > 90 || driverLocations.Longitude < -180 || driverLocations.Longitude > 180)
                throw new HubException("Invalid latitude or longitude.");

            // call Update driver location service 
            await _updateLocation.UpdateDriverLocationAsync(driver.Id, driverLocations.Latitude, driverLocations.Longitude, driver.Status, user.Gender , vehicleType);
            await Clients.All.SendAsync("ReceiveLocationUpdate", driver.Id, driverLocations.Latitude, driverLocations.Longitude);

        }
    }
}
