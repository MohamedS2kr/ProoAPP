using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Passenger : BaseEntity
    {
      
        public ApplicationUser User { get; set; }
        public string PreferredPaymentMethod { get; set; }
        public ICollection<Ride> Rides { get; set; }
    }
}
