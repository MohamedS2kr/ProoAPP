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
            #region test
            //switch (vehicleCategory.ToLower())
            //{
            //    case "economy":
            //        baseFare = 5;
            //        initialDistanceRate = 2;
            //        extraDistanceRate = 3;
            //        timeRate = 0.5m;
            //        surgeMultiplier = 1.2m;
            //        break;
            //    case "luxury":
            //        baseFare = 10;
            //        initialDistanceRate = 4;
            //        extraDistanceRate = 5;
            //        timeRate = 1;
            //        surgeMultiplier = 1.5m;
            //        break;
            //    default:
            //        throw new ArgumentException("Invalid vehicle category.");
            //}

            //// حساب التكلفة
            //var totalFare = baseFare;

            //if (distance <= extraDistanceThreshold)
            //{
            //    totalFare += distance * initialDistanceRate;
            //}
            //else
            //{
            //    totalFare += (extraDistanceThreshold * initialDistanceRate) + ((distance - extraDistanceThreshold) * extraDistanceRate);
            //}

            //totalFare += duration * timeRate;
            //totalFare *= surgeMultiplier;

            //return totalFare;

            #region new version
            //if (distanceInKm <= shortDistanceLimit)
            //{
            //    return baseFare;
            //}

            //decimal totalFare = 0;
            //decimal remainingDistance = distanceInKm;

            //foreach (var (distanceLimit, farePerKm) in fareTiers)
            //{
            //    var tierDistance = Math.Min(remainingDistance, distanceLimit);
            //    totalFare += tierDistance * farePerKm;
            //    remainingDistance -= tierDistance;

            //    if (remainingDistance <= 0)
            //        break;
            //}
            #endregion

            #endregion

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
