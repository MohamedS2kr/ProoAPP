using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities.Price_Estimate
{
    public class PriceCategoryTier
    {
        public int Id { get; set; }

        public string CategoryName { get; set; }
        public int? PriceEstimatedPlanId { get; set; }
        public virtual PriceEstimatedPlan? PriceEstimatedPlan { get; set; }
        public virtual List<PricePerDistance>? PricePerDistances { get; set; }
    }
}
