using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Identity
{
    public class LoginDto
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        //[Required]
        //public string MacAddress { get; set; }
    }
}
