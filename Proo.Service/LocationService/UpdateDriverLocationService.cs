using Proo.Core.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.LocationService
{
    public class UpdateDriverLocationService : IUpdateDriverLocationService
    {
        private readonly IDatabase _database;
        private const string driverLocationKey = "driver:locations";
        public UpdateDriverLocationService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task UpdateDriverLocationAsync(string driverId, double lat, double Long , DriverStatus driverStatus , Gender DriverGender , string vehicleType)
        {
            // Store driver location using GeoAdd
            await _database.GeoAddAsync(driverLocationKey, new GeoEntry(Long, lat, driverId));

            // Store driver status to indicate they are active
            string driverStatusKey = $"driver:status:{driverId}";

            await _database.HashSetAsync(driverStatusKey, new HashEntry[]
            {
                new HashEntry("status" , driverStatus.ToString()),
                new HashEntry("lastUpdateTime" , DateTime.Now.ToString("o")),
                new HashEntry("driverGender" , DriverGender.ToString()),
                new HashEntry("driverVehicleType" , vehicleType)
            });

            // Set a TTL on the driver status to indicate availability expires if no update is received
            await _database.KeyExpireAsync(driverStatusKey, TimeSpan.FromMinutes(5));
        }
    }
}
