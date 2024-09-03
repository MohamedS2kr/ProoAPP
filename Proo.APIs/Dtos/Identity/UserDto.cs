namespace Proo.APIs.Dtos.Identity
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
        public IEnumerable<string> Role { get; set; } = new List<string>();
    }
}
