using Proo.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Identity
{
    public class DriverDto
    {
        [Required]
        public IFormFile LicenseIdFront { get; set; } 

        [Required]
        public IFormFile LicenseIdBack { get; set; }

        [Required]
        public DateTime ExpiringDate { get; set; } = DateTime.Now;
       
        public bool IsAvailable { get; set; }
        
    }
}
