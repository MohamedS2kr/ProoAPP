using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public enum Gender
    {
        Male,
        Female
    }
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }

    }
}
