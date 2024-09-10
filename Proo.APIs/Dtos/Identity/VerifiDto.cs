using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Identity
{
    public class VerifiDto
    {
        [Required]
        public string Otp { get; set; }


        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
   

}
