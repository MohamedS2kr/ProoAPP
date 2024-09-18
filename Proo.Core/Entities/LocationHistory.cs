﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class LocationHistory : BaseEntity
    {
        public string Id { get; set; }
        public string RideId { get; set; }
        public Ride Ride { get; set; }
        public DateTime Timestamp { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
