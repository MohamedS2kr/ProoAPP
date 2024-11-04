using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Entities.Driver_Location;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.Nearby_Driver_Service
{
    public class NearbyDriversService : INearbyDriverService
    {
        private readonly IDatabase _database;
        private readonly IDriverRepository _driverRepo;
        private const string DriverGeoKey = "driver:locations";
        public NearbyDriversService(IConnectionMultiplexer redis , IDriverRepository driverRepo)
        {
            _database = redis.GetDatabase();
            _driverRepo = driverRepo;
        }

        public async Task<List<Guid>> GetNearbyAvailableDriversAsync(double pickupLat, double pickupLng, double radiusKm, int maxDrivers , string vehicleCategory, string GenderType )
        {
            // Get driver IDs within the given radius
            //var nearbyDrivers = await _database.GeoRadiusAsync(DriverGeoKey, pickupLng, pickupLat, radiusKm * 1000, GeoUnit.Kilometers, count: maxDrivers, order : Order.Ascending);
            var nearbyDrivers = await _database.GeoSearchAsync(
                key: DriverGeoKey,
                longitude: pickupLng,
                latitude: pickupLat,
                shape: new GeoSearchCircle(radiusKm, GeoUnit.Kilometers),
                order: Order.Ascending, 
                count: maxDrivers 
                );

            var aviableDriverIds = new List<Guid>();

            foreach (var driver in nearbyDrivers)
            {
                var driverId = Guid.Parse(driver.Member);

                // Check if the driver is available by querying their status
                var status = await _database.HashGetAsync($"driver:status:{driverId}", "status");
                var driverGender = await _database.HashGetAsync($"driver:status:{driverId}", "driverGender");
                var driverVehicleCategory = await _database.HashGetAsync($"driver:status:{driverId}", "vehicleCategory");


                if (GenderType == "0") // femaleOnly
                {
                    if (status.HasValue && status.ToString() == "Available" &&
                        driverGender.HasValue && driverGender.ToString() == "Female" &&
                        driverVehicleCategory.HasValue && driverVehicleCategory.ToString() == vehicleCategory)
                    {
                        aviableDriverIds.Add(driverId);
                    }
                }
                else 
                {
                    if (status.HasValue && status.ToString() == "Available" &&
                        driverVehicleCategory.HasValue && driverVehicleCategory.ToString() == vehicleCategory)
                    {
                        aviableDriverIds.Add(driverId);
                    }
                }

                if (aviableDriverIds.Count >= maxDrivers)
                    break;
            }

            return aviableDriverIds;
        }

        public async Task<List<DriverLocations>> GetAllNearbyAvailableDriversAsync(double pickupLat, double pickupLng, double radiusKm, int maxDrivers)
        {
            var nearbyDrivers = await _database.GeoSearchAsync(
               key: DriverGeoKey,
               longitude: pickupLng,
               latitude: pickupLat,
               shape: new GeoSearchCircle(radiusKm, GeoUnit.Kilometers),
               order: Order.Ascending,
               count: maxDrivers
               );

            var driverList = new List<DriverLocations>();

            foreach (var driver in nearbyDrivers)
            {
                var vehicleType = await GetVehicleTypeByDriverId(driver.Member , _driverRepo);

                driverList.Add(new DriverLocations
                {
                    DriverId = driver.Member,
                    Longitude = driver.Position.Value.Longitude,
                    Latitude = driver.Position.Value.Latitude,
                    VehicleType = vehicleType
                });

            }


            return driverList;
        }

        private async Task<string> GetVehicleTypeByDriverId(string driverId , IDriverRepository _driverRepo)
        {

            var driverDetails = await _driverRepo.GetDriverOrPassengerByIdAsync(driverId);

            // Check if there are any vehicles and return the type name of the first vehicle
            var vehicleTypeName = driverDetails?.Vehicles?.FirstOrDefault()?.vehicleModel?.VehicleType?.TypeName;

            return vehicleTypeName; // Return the vehicle type name or null if not found
        }

    }
}
