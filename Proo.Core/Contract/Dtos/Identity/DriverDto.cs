using Proo.Core.Contract.Dtos.Rides;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.Dtos.Identity
{
    public class DriverDto
    {
        public string Id { get; set; }

        public string NationalIdFront { get; set; }
        public string NationalIdBack { get; set; }
        public DateTime NationalIdExpiringDate { get; set; } = DateTime.Now;
        public string DrivingLicenseIdFront { get; set; }
        public string DrivingLicenseIdBack { get; set; }
        public DateTime DrivingLicenseExpiringDate { get; set; } = DateTime.Now;
        public DateTime LastActiveTime { get; set; }
        public DriverStatusWork StatusWork { get; set; } = DriverStatusWork.Pending; // Defualt
        public DriverStatus Status { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<VehicleDto>? Vehicles { get; set; }
        public virtual ICollection<RideDto>? Rides { get; set; }
    }
}
