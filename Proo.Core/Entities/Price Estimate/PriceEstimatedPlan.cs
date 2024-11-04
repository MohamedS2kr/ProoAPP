using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities.Price_Estimate
{
    public class PriceEstimatedPlan
    {
        public int Id { get; set; }
        public decimal basePrice { get; set; }
        public decimal shortDistanceLimit { get; set; }
        public decimal shortDistancePrice { get; set; }
        // List of price tiers linked to this plan
        public virtual List<PriceCategoryTier>? CategoryPriceTiers { get; set; }
    }
}
