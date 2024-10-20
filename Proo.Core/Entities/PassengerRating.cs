using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class PassengerRating : BaseEntity
    {
        public int Id { get; set; }
        public int Score { get; set; } // Rating from 1 to 5 [rang dataAnnotation]
        public string? Review { get; set; }
        public string PassengerId { get; set; }
        public virtual Passenger? Passenger { get; set; }
        public int RideId { get; set; }
        public virtual Ride Ride { get; set; }
    }
}
