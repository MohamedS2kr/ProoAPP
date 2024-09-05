using Proo.Core.Entities;

namespace Proo.APIs.Dtos.Identity
{
    public class DriverDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public IFormFile LicenseIdFront { get; set; }
        public IFormFile LicenseIdBack { get; set; }
        public DateTime ExpiringDate { get; set; }
       

        public IFormFile VehicleLicenseIdFront { get; set; }
        public IFormFile VehicleLicenseIdBack { get; set; }
        public DateTime VehicleExpiringDate { get; set; }

        public string Type { get; set; }
        public string category { get; set; }
        public DateTime YeareOfManufacuter { get; set; }
        public bool AirConditional { get; set; }
        public int NumberOfPassenger { get; set; }
        public int NumberOfPalet { get; set; }
        public string Colour { get; set; }
        public bool IsAvailable { get; set; }
    }
}
