using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Identity
{
    public class ResendOtpDto
    {
        [Required]
        [RegularExpression(@"^01[0-25]\d{8}$", ErrorMessage = "Invalid phone number. Please enter a valid Egyptian phone number in the format +201234567890 or 01234567890.")]
        public string PhoneNumber { get; set; }
    }
}
