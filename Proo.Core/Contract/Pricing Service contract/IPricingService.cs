using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.Pricing_Service_contract
{
    public interface IPricingService
    {
        Task<object> CalculateTripPriceAsync(string vehicleType, double distance);
    }
}
