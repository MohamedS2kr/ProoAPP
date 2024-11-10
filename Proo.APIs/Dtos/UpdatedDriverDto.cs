using Proo.Core.Entities;
using System.ComponentModel.DataAnnotations;
#nullable enable
namespace Proo.APIs.Dtos
{
    public class UpdatedDriverDto
    {
        [MaxLength(100, ErrorMessage = "The max length is 100 char")]
        [MinLength(5, ErrorMessage = "The min length is 5 char")]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public IFormFile? UploadFile { get; set; }
        //public string Role { get; set; }

        public Gender Gender { get; set; }

        public DateTime DataOfBirth { get; set; } = DateTime.Now;
        public IFormFile? LicenseIdFront { get; set; }

   
        public IFormFile? LicenseIdBack { get; set; }

     
        public DateTime ExpiringDate { get; set; } = DateTime.Now;

        public bool IsAvailable { get; set; } = false;
    }
}
