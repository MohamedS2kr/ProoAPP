using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.Dtos.Passenger
{
    public class PassengerRatingDto
    {
        public int Id { get; set; }
        public int Score { get; set; } // Rating from 1 to 5 [rang dataAnnotation]
        public string? Review { get; set; }
        public string PassengerId { get; set; }
        public int RideId { get; set; }
    }
}
