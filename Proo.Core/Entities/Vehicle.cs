using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Vehicle : BaseEntity
    {
        public int Id { get; set; }
     
        public DateTime YeareOfManufacuter { get; set; }
        public bool AirConditional { get; set; }
        public int NumberOfPassenger { get; set; }
        public int NumberOfPlate { get; set; }
        public string Colour { get; set; }

        public string VehicleLicenseIdFront { get; set; }
        public string VehicleLicenseIdBack { get; set; }
        public DateTime ExpiringDateOfVehicleLicence { get; set; }


        //navigation property for Driver
        public string DriverId { get; set; }
        public Driver? Driver { get; set; }

        // Foreign key to model
        public int VehicleModelId { get; set; }
        public VehicleModel? vehicleModel { get; set; }
    }
}
