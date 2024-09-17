using Proo.Core.Entities;

namespace Proo.APIs.Dtos.Identity
{
    public class UserDto
    {
        public string FullName { get; set; }
        
        public string PhoneNumber { get; set; }
        public Gender Gender { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Role { get; set; } = new List<string>();
    }
}
