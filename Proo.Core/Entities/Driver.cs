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
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string? LicenseIdFront { get; set; }
        public string? LicenseIdBack { get; set; }
        public DateTime ExpiringDate { get; set; } = DateTime.Now;
        //public string VehicleDetails { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsVerified { get; set; } //( هل السائق موثق (تحقق من الهوية مثلاً
        public ICollection<Ride> Rides { get; set; }
        public DriverStatus Status { get; set; } = DriverStatus.Pending; // Defualt
    }

    public enum DriverStatus
    {
        Pending,
        Approved,
        Rejected
    }

}
