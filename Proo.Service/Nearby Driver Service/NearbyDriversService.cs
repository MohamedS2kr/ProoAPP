using Proo.Core.Contract.Nearby_driver_service_contract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.Nearby_Driver_Service
{
    public class NearbyDriversService : INearbyDriversService
    {
        private readonly IDatabase _database;
        private const string DriverGeoKey = "driver:locations";
        public NearbyDriversService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<List<Guid>> GetNearbyAvailableDriversAsync(double pickupLat, double pickupLng, double radiusKm, int maxDrivers , string GenderType)
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

                if(GenderType == "0") // femaleOnly
                {
                    if (status.HasValue && status.ToString() == "Avaiable" && driverGender.HasValue && driverGender == "Female")
                    {
                        aviableDriverIds.Add(driverId);
                    }

                    // Stop if the maximum number of drivers is reached
                    if (aviableDriverIds.Count >= maxDrivers)
                        break;
                }

                if (status.HasValue && status.ToString() == "Avaiable")
                {
                    aviableDriverIds.Add(driverId);
                }

                // Stop if the maximum number of drivers is reached
                if (aviableDriverIds.Count >= maxDrivers)
                    break;
            }

            return aviableDriverIds;
        }
    }
}
