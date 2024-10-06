using Proo.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Proo.APIs.Dtos.Identity
{
    public class DriverToReturnDto
    {
     
        public string FullName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }

        public IEnumerable<string> Role { get; set; } = new List<string>();

        public Gender Gender { get; set; }

        public string ProfilePictureUrl { get; set; }


        public string NationalIdFront { get; set; }

        public string NationalIdBack { get; set; }
        public DateTime NationalIdExpiringDate { get; set; }

        public string DrivingLicenseIdFront { get; set; }

        public string DrivingLicenseIdBack { get; set; }

  
        public DateTime ExpiringDateOfDrivingLicense { get; set; }


        // Foreign keys for vehicle relations (Dropdowns for selection in UI)

        public string VehicleModel { get; set; }
        public string VehicleType { get; set; }
 
        public string VehicleCategory { get; set; }

        public string YeareOfManufacuter { get; set; }
        public bool AirConditional { get; set; }
        public int NumberOfPassenger { get; set; }
        public string NumberOfPlate { get; set; }
        public string Colour { get; set; }
        public string ColourHexa { get; set; }
        public string VehiclePicture { get; set; }

        public string VehicleLicenseIdFront { get; set; }
   
        public string VehicleLicenseIdBack { get; set; }

        public DateTime VehicleExpiringDate { get; set; }
        public string Token { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiredation { get; set; }
    }
}
