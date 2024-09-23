﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public abstract class Rating : BaseEntity
    {
        public int Id { get; set; }

        public int Score { get; set; } // Rating from 1 to 5 [rang dataAnnotation]

        public string? Review { get; set; }
        public DateTime Date { get; set; }

        public string PassengerId { get; set; } = string.Empty;
        public Passenger? Passenger { get; set; }
        public string DriverId { get; set; } = string.Empty;
        public Driver? Driver { get; set; }

        public int RideId { get; set; }
        public Ride? Ride { get; set; }
    }
}
