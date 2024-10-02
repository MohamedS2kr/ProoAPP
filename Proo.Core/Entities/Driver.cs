using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Driver : BaseEntity
    {
        public string Id { get; set; }

        public string NationalIdFront { get; set; }
        public string NationalIdBack { get; set; }
        public string DrivingLicenseIdFront { get; set; }
        public string DrivingLicenseIdBack { get; set; }
        public DateTime DrivingLicenseExpiringDate { get; set; } = DateTime.Now;
        public DateTime LastActiveTime { get; set; }
        public DriverStatusWork StatusWork { get; set; } = DriverStatusWork.Pending; // Defualt
        public DriverStatus Status { get; set; } 

        public string UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public ICollection<Vehicle>? Vehicles { get; set; }
        public  ICollection<PassengerRating>? PassengerRatings { get; private set; }
        public  ICollection<DriverRating>? DriverRatings { get; private set; }
        public ICollection<Ride>? Rides { get; set; }


    }

    public enum DriverStatusWork
    {
        Pending,
        Approved,
        Rejected
    }

    public enum DriverStatus
    {

        Avaiable,
        InRide,
        Offline
    }

}
