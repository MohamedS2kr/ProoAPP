using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class RideRequests :BaseEntity
    {
        public int Id { get; set; }//pK

        public string PickupAddress { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public string DropoffAddress { get; set; }
        public double DropoffLatitude { get; set; }
        public double DropoffLongitude { get; set; }
        
        public double EstimatedDistance { get; set; }
        public double EstimatedTime { get; set; } // Estimated time in minutes
        public double EstimatedPrice { get; set; }
        public string Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
        public Ride Ride { get; set; }
        public string PassengerId { get; set; }
        public Passenger Passenger { get; set; }
    }

    public enum Status
    {
        Pending,
        Accepted,
        Rejected,
        Cancelled
    }
}
