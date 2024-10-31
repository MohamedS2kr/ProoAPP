using Proo.Core.Contract.Pricing_Service_contract;
using Proo.Core.Entities.Price_Estimate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.Pricing_Service
{
    public class PricingService : IPricingService
    {
        private readonly List<PriceEstimate> _priceEstimates;

        public PricingService()
        {
            _priceEstimates = new List<PriceEstimate>()
            {
                new PriceEstimate { VehicleType = "Standard", BasePricePerKilo = 1.00, IncreasedPricePerKilo = 1.50, ThresholdMiles = 10 },
                new PriceEstimate { VehicleType = "Luxury", BasePricePerKilo = 2.00, IncreasedPricePerKilo = 3.00, ThresholdMiles = 15 },
                new PriceEstimate { VehicleType = "Economy", BasePricePerKilo = 0.75, IncreasedPricePerKilo = 1.00, ThresholdMiles = 5 }
            };
        }
        public async Task<object> CalculateTripPriceAsync(string vehicleType, double distance)
        {
            var pricingTier = _priceEstimates.FirstOrDefault(p => p.VehicleType.Equals(vehicleType, StringComparison.OrdinalIgnoreCase));

            if (pricingTier == null)
                throw new ArgumentException("Invalid vehicle type");

            double price = 0.0;

            if (distance <= pricingTier.ThresholdMiles)
            {
                price = pricingTier.BasePricePerKilo * distance; // حساب السعر للمسافة العادية
            }
            else
            {
                // حساب السعر للمسافة التي تتجاوز الحد
                double normalKilos = pricingTier.ThresholdMiles;
                double extraKilos = distance - normalKilos;
                price = (pricingTier.BasePricePerKilo * normalKilos) + (pricingTier.IncreasedPricePerKilo * extraKilos);
            }

            return await Task.FromResult(price);
        
        }
    }
}
