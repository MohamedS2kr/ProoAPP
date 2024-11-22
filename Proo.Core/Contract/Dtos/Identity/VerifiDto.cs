using System.ComponentModel.DataAnnotations;

namespace Proo.Core.Contract.Dtos.Identity
{
    public class VerifiDto
    {
        [Required]
        public string Otp { get; set; }


        [Required]
        [Phone]
        [RegularExpression(@"^01[0-25]\d{8}$", ErrorMessage = "Invalid phone number. Please enter a valid Egyptian phone number in the format +201234567890 or 01234567890.")]
        public string PhoneNumber { get; set; }
    }
   

}
