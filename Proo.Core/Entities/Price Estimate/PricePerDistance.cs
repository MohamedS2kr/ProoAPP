using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities.Price_Estimate
{
    public class PricePerDistance
    {
        public int Id { get; set; }

        // الحد الأقصى للمسافة
        public decimal DistanceLimit { get; set; }

        // الأجرة لكل كيلومتر
        public decimal PricePerKm { get; set; }
        public int? PriceCategoryTierId { get; set; }
        public virtual PriceCategoryTier? PriceCategoryTier { get; set; }
    }
}
