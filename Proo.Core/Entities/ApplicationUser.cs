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
        public bool IsPhoneNumberConfirmed { get; set; } = false;
        public bool IsOtpValid { get; set; }
        public string? OtpCode { get; set; } // Store the OTP
        public DateTime? OtpExpiryTime { get; set; } // OTP expiration time
        public string? FullName { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime? DateOfBirth { get; set; } = DateTime.Now;
        public Gender Gender { get; set; } = Gender.Male;
        public string? MacAddress { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }

    }
}
