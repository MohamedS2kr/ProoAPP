using Proo.Core.Entities;

namespace Proo.APIs.Dtos.Identity
{
    public class DriverDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string LicenseIdFront { get; set; }
        public string LicenseIdBack { get; set; }
        public DateTime ExpiringDate { get; set; }
        public string Type { get; set; }
        public string category { get; set; }
        public DateTime YeareOfManufacuter { get; set; }
        public bool AirConditional { get; set; }
        public int NumberOfPassenger { get; set; }
        public int NumberOfPalet { get; set; }
        public string Colour { get; set; }

        public string VehicleLicenseIdFront { get; set; }
        public string VehicleLicenseIdBack { get; set; }
        public DateTime VehicleExpiringDate { get; set; }
        public bool IsAvailable { get; set; }
    }
}
