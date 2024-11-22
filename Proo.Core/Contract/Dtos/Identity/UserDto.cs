using Proo.Core.Entities;
using System.Text.Json.Serialization;

namespace Proo.Core.Contract.Dtos.Identity
{
    public class UserDto
    {
        public string FullName { get; set; }
        
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public IEnumerable<string> Role { get; set; } = new List<string>();
        public string Token { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiredation { get; set; }
    }
}
