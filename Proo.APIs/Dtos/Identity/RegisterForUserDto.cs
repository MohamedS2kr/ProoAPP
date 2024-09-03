using Proo.Core.Entities;
using System;

namespace Proo.APIs.Dtos.Identity
{
    public class RegisterForUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DataOfBirth { get; set; }
    }
    
}
