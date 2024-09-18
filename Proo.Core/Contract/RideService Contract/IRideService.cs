using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.RideService_Contract
{
    public interface IRideService
    {

        Task<IReadOnlyList<Driver>> GetNearbyDrivers(double pickuplat , double pickuplong , double radiusKm);
    }
}
