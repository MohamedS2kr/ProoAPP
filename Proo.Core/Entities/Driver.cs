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
        public ApplicationUser User { get; set; }
        public string UserId { get; set; }
        public string LicenseIdFront { get; set; }
        public string LicenseIdBack { get; set; }
        public DateTime ExpiringDate { get; set; }
        //public string VehicleDetails { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<Ride> Rides { get; set; }
    }
}
