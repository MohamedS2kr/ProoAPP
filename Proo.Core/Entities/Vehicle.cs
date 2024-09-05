using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Vehicle : BaseEntity
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string category { get; set; }
        public DateTime YeareOfManufacuter { get; set; }
        public bool AirConditional { get; set; }
        public int NumberOfPassenger { get; set; }
        public int NumberOfPalet { get; set; }
        public string Colour { get; set; }

        public string VehicleLicenseIdFront { get; set; }
        public string VehicleLicenseIdBack { get; set; }
        public DateTime ExpiringDate { get; set; }
        public Driver Driver { get; set; }
        public string DriverId { get; set; }
    }
}
