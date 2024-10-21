using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class AreaPricingRange:BaseEntity
    {
        public int PricingPlanId { get; set; }
        public decimal AdditionalPricePerKilometer { get; set; }
        public int StartRangeInKilometers { get; set; }
        public int EndRangeInKilometers { get; set; }
        public PricingPlan PricingPlan { get; set; }
    }
}
