using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.Nearby_driver_service_contract
{
    public interface INearbyDriversService
    {
        Task<List<Guid>> GetNearbyAvailableDriversAsync(double pickupLat, double pickupLng, double radiusKm, int maxDrivers , string GenderType);
    }
}
