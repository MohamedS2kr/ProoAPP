using Proo.Core.Entities.Driver_Location;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.Nearby_Driver_Service
{
    public interface INearbyDriverService
    {
        Task<List<Guid>> GetNearbyAvailableDriversAsync(double pickupLat, double pickupLng, double radiusKm, int maxDrivers , string vehicleCategory, string GenderType = "1");
        Task<List<DriverLocations>> GetAllNearbyAvailableDriversAsync(double pickupLat, double pickupLng, double radiusKm, int maxDrivers);
        
    }
}
