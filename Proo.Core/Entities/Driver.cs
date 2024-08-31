using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Driver
    {
        public string Id { get; set; }
        public ApplicationUser User { get; set; }
        public string LicenseNumber { get; set; }
        public string VehicleDetails { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<Ride> Rides { get; set; }
    }
}
