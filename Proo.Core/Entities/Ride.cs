using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Ride
    {
        public int Id { get; set; }
        public string PassengerId { get; set; }
        public Passenger Passenger { get; set; }
        public string DriverId { get; set; }
        public Driver Driver { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PickupLocation { get; set; }
        public string DropoffLocation { get; set; }
        public decimal Fare { get; set; }
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
