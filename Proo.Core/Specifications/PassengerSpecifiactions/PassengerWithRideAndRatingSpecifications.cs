using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Specifications.PassengerSpecifiactions
{
    public class PassengerWithRideAndRatingSpecifications:BaseSpecifications<Ride>
    {
        public PassengerWithRideAndRatingSpecifications(string passengerId):base(r => r.PassengerId == passengerId)
        {
           
            AddOrderByDesc(r => r.CompletedAt);
        }
    }
}
