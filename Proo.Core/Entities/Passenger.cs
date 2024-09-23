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
        public ApplicationUser User { get; set; }
        public string PreferredPaymentMethod { get; set; }
        public ICollection<Ride> Rides { get; set; } 
        //public ICollection<RideRequests> RideRequests { get; set; }
        public ICollection<PassengerRating> PassengerRatings { get; private set; }
        public ICollection<DriverRating> DriverRatings { get; private set; }
        public bool IsRiding { get; set; }
    }
}
