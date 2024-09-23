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
        public string PassengerId { get; set; }
        public virtual Passenger? Customer { get; set; }
        public string? DriverId { get; set; }
        public virtual Driver? Driver { get; set; }
        public RideRequestStatus Status { get; set; }
        public PaymentMethod paymentMethod { get; set; }
    }

    public enum RideRequestStatus
    {
        NO_DRIVER_FOUND = 1,
        CUSTOMER_CANCELED = 2,
        DRIVER_ACCEPTED = 3,
        CUSTOMER_REJECTED_DRIVER = 4,
        DRIVER_REJECTED_CUSTOMER = 5, // TODO: upon 3 driver rejections, take the trip request to trip_request_rejected
        TRIP_STARTED = 6, // create a trip entity where status reaches this stage
        TRIP_REQUEST_REJECTED = 7 // TODO: run a scheduler and send all unaccepted trips to this status
    }
}
