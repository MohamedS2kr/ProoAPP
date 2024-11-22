using System.Text.Json.Serialization;

namespace Proo.Core.Contract.Dtos.Identity
{
    public class LoginToreturnDto
    {
        public string Token { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public string Otp { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiredation { get; set; }
    }
}
