using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Proo.Core.Entities;
using Proo.Core.Entities.Driver_Location;
using Proo.Infrastructer.Repositories.DriverRepository;
using Proo.Service.LocationService;
using Proo.Service.Nearby_Driver_Service;
using StackExchange.Redis;
using System.Security.Claims;

namespace Proo.APIs.Hubs
{
    public class LocationHub : Hub
    {
        private readonly IUpdateDriverLocationService _updateLocation;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DriverRepository _driverRepo;
        private readonly INearbyDriverService _nearbyDriverService;

        public LocationHub(IUpdateDriverLocationService updateLocation 
            , UserManager<ApplicationUser> userManager
            , DriverRepository driverRepo
            , INearbyDriverService nearbyDriverService)
        {
            _updateLocation = updateLocation;
            _userManager = userManager;
            _driverRepo = driverRepo;
            _nearbyDriverService = nearbyDriverService;
        }
        public async Task UpdateLocationDriver(double Latitude , double Longitude)
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
            var vehicleCategory = vehicles.Select(v => v.vehicleModel?.VehicleType?.CategoryOfVehicle?.Name).FirstOrDefault(); 

            //if (driverLocations is null /*|| driverLocations.DriverId != driver.Id*/)
            //    throw new HubException("Invalid request data.");
            if (Latitude < -90 || Latitude > 90 || Longitude < -180 || Longitude > 180)
                throw new HubException("Invalid latitude or longitude.");

            // call Update driver location service 
            await _updateLocation.UpdateDriverLocationAsync(driver.Id, Latitude, Longitude, driver.Status, user.Gender , vehicleType , vehicleCategory);

            // إرسال الموقع المحدث إلى جميع العملاء
            //await Clients.All.SendAsync("ReceiveLocationUpdate", driver.Id, Latitude, Longitude);

        }

        public async Task GetNearbyDrivers(double userLatitude, double userLongitude)
        {
            var nearbyDriver = await _nearbyDriverService.GetAllNearbyAvailableDriversAsync(userLatitude, userLongitude , 20 , 50);

            await Clients.Caller.SendAsync("ReceiveNearbyDrivers", nearbyDriver);
        }
    }
}
