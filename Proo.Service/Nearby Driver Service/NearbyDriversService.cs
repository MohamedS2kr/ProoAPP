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

        public async Task<List<Guid>> GetNearbyAvailableDriversAsync(double pickupLat, double pickupLng, double radiusKm, int maxDrivers)
        {
            // Get driver IDs within the given radius
            var nearbyDrivers = await _database.GeoRadiusAsync(DriverGeoKey, pickupLng, pickupLat, radiusKm * 1000, GeoUnit.Meters, count: maxDrivers, order : Order.Ascending);

            var aviableDriverIds = new List<Guid>();

            foreach (var driver in nearbyDrivers)
            {
                var driverId = Guid.Parse(driver.Member);

                // Check if the driver is available by querying their status
                var status = await _database.HashGetAsync($"driver:status:{driverId}", "status");
                if (status.HasValue && status.ToString() == "Available")
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
