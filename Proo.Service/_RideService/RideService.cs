using Proo.Core.Contract;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Core.Specifications.DriverSpecifiactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service._RideService
{
    public class RideService : IRideService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RideService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       

        public async Task<IReadOnlyList<Driver>> GetNearbyDrivers(double pickuplat, double pickuplong, double radiusKm)
        {
            var spec = new DriverWithApplicationUserSpecifiaction();
            var AllDrivers = await _unitOfWork.Repositoy<Driver>().GetAllWithSpecAsync(spec);
            return AllDrivers.Where(d =>
            {
            
                var lastRide = d.Rides.OrderByDescending(r => r.Id).FirstOrDefault();
                if (lastRide == null) return false; 

                // Calculate distance using Haversine formula
                return new LocationService().HaversineDistance(pickuplat, lastRide.DestinationLocation.Latitude, pickuplong , lastRide.DestinationLocation.Longitude) <= radiusKm;
            }).ToList();
        }
    }
}
