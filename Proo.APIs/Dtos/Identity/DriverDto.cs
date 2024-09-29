using Proo.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Identity
{
    public class DriverDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "The max length is 100 char")]
        [MinLength(5, ErrorMessage = "The min length is 5 char")]
        public string FullName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string Role { get; set; }
        [Required]
        public Gender Gender { get; set; }


        [Required]
        public IFormFile NationalIdFront { get; set; }
        [Required]
        public IFormFile NationalIdBack { get; set; }

        [Required]
        public IFormFile LicenseIdFront { get; set; } 

        [Required]
        public IFormFile LicenseIdBack { get; set; }

        [Required]
        public DateTime ExpiringDate { get; set; } = DateTime.Now;
       
        public bool IsAvailable { get; set; }

        // Foreign keys for vehicle relations (Dropdowns for selection in UI)
       

        [Required]
        public int VehicleModelId { get; set; } 

        [Required]
        public DateTime YeareOfManufacuter { get; set; }
        [Required]
        public bool AirConditional { get; set; }
        [Required]
        public int NumberOfPassenger { get; set; }
        [Required]
        public int NumberOfPalet { get; set; }
        [Required]
        public string Colour { get; set; }
        [Required]
        public IFormFile VehicleLicenseIdFront { get; set; }
        [Required]
        public IFormFile VehicleLicenseIdBack { get; set; }
        [Required]
        public DateTime VehicleExpiringDate { get; set; }

    }
}
