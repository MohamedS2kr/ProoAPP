﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace Proo.Core.Entities
{
    public class Vehicle : BaseEntity
    {
        public int Id { get; set; }
     
        public string YeareOfManufacuter { get; set; }
        public bool AirConditional { get; set; }
        public int NumberOfPassenger { get; set; }
        public string NumberOfPlate { get; set; }
        public string Colour { get; set; }
        public string VehiclePicture { get; set; }
        public string VehicleLicenseIdFront { get; set; }
        public string VehicleLicenseIdBack { get; set; }
        public DateTime ExpiringDateOfVehicleLicence { get; set; }


        //navigation property for Driver
        public string DriverId { get; set; }
        public virtual Driver? Driver { get; set; }

        // Foreign key to model
        public int VehicleModelId { get; set; }
        public virtual VehicleModel? vehicleModel { get; set; }
    }
}
