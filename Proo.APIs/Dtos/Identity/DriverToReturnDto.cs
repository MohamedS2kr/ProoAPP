using Proo.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Identity
{
    public class DriverToReturnDto
    {
     
        public string FullName { get; set; }

 
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }

        public IEnumerable<string> Role { get; set; } = new List<string>();

        public Gender Gender { get; set; }


        public string? LicenseIdFront { get; set; }

    
        public string? LicenseIdBack { get; set; }

     
        public DateTime ExpiringDate { get; set; }

        public bool IsAvailable { get; set; }

        public string Type { get; set; }
        
        public string category { get; set; }

      
        public DateTime YeareOfManufacuter { get; set; }
    
        public bool AirConditional { get; set; }
      
        public int NumberOfPassenger { get; set; }
     
        public int NumberOfPalet { get; set; }
    
        public string ColourHexa { get; set; }
        public string Colour { get; set; }
      
        public string? VehicleLicenseIdFront { get; set; }
    
        public string? VehicleLicenseIdBack { get; set; }
      
        public DateTime VehicleExpiringDate { get; set; }
        public string Token { get; set; }
    }
}
