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
        public DateTime CreatedAt { get;  set; }
        public DateTime? LastModifiedAt { get; set; }
        public DateTime? DeletedAt { get;  set; }
        public string PassengerId { get; set; }
        public  Passenger? Passenger { get; set; }
        public string? DriverId { get; set; }
        public  Driver? Driver { get; set; }
        public RideRequestStatus Status { get; set; } 
        public PaymentMethod paymentMethod { get; set; }
    }

    public enum RideRequestStatus
    {
        NO_DRIVER_FOUND = 1,
        Requested = 2 ,
        CUSTOMER_CANCELED = 3,
        CUSTOMER_ACCEPTED = 4,
        CUSTOMER_REJECTED_DRIVER = 5,
        TRIP_STARTED = 6, // create a trip entity where status reaches this stage
    }
}
