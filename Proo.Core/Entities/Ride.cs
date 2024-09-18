using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Ride : BaseEntity
    {
        public int Id { get; set; }
        public string PassengerId { get; set; }
        public Passenger Passenger { get; set; }
        public string DriverId { get; set; }
        public Driver Driver { get; set; }

        public Locations PickupLocation { get; set; }
        public Locations DestinationLocation { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public decimal FarePrice { get; set; }
        public RideStatus Status { get; set; }
    }


    public enum RideStatus
    {
        Requested,
        Accepted,
        InProgress,
        Completed,
        Cancelled
    }
}
