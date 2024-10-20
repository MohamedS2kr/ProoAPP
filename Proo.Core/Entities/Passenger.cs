using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Passenger : BaseEntity
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string? PreferredPaymentMethod { get; set; }
        public virtual ICollection<Ride>? Rides { get; set; } 
        //public ICollection<RideRequests> RideRequests { get; set; }
        public virtual ICollection<PassengerRating>? PassengerRatings { get; private set; }
        
        public bool IsRiding { get; set; }
    }
}
