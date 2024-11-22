using Proo.Core.Contract.Dtos.Rides;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.Dtos.Passenger
{
    public class PassengerDto
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string? PreferredPaymentMethod { get; set; }
        public virtual ICollection<RideDto>? Rides { get; set; }
        //public ICollection<RideRequests> RideRequests { get; set; }
        public virtual ICollection<PassengerRatingDto>? PassengerRatings { get; private set; }

        public bool IsRiding { get; set; }
    }
}
