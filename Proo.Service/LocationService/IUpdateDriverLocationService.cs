using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.LocationService
{
    public interface IUpdateDriverLocationService
    {
        Task UpdateDriverLocationAsync(string driverId, double lat, double Long , DriverStatus driverStatus ,Gender DriverGender);
    }
}
