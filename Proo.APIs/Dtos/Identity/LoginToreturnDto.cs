using System.Text.Json.Serialization;

namespace Proo.APIs.Dtos.Identity
{
    public class LoginToreturnDto
    {
        public string Token { get; set; }
        public string Otp { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiredation { get; set; }
    }
}
