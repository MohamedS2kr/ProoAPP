using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities.Price_Estimate
{
    public class PriceEstimate
    {
        public string tripCategory { get; set; }
        public string VehicleType { get; set; }
        public string PriceRange { get; set; } //  "$10 - $15"
        public double LowEstimate { get; set; }
        public double HighEstimate { get; set; }
        public double SurgeMultiplier { get; set; }
        public string EstimatedTime { get; set; } // Estimated time of arrival




        public double BasePricePerKilo { get; set; } // سعر الأساس لكل kilo
        public double IncreasedPricePerKilo { get; set; } // السعر المعزز لكل kilo
        public double ThresholdMiles { get; set; } // الحد الأقصى للمسافة قبل تغيير السعر
    }
}
