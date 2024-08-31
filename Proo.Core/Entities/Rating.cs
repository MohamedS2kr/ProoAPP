using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Rating
    {
        public int Id { get; set; }
        public int RideId { get; set; }
        public Ride Ride { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int Score { get; set; } // Rating from 1 to 5 [rang dataAnnotation]

        public string Review { get; set; }
        public DateTime Date { get; set; }
    }
}
