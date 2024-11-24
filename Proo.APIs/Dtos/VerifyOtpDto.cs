using Proo.APIs.Dtos.Identity;
using Proo.APIs.Dtos.Passenger;
using System.Text.Json.Serialization;

namespace Proo.APIs.Dtos
{
    public class VerifyOtpDto
    {
        public string Token { get; set; }
        public DriverToReturnDto Profile { get; set; }
    }
}
