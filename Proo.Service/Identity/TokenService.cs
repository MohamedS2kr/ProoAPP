using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;

namespace Proo.Service.Identity
{
    public class TokenServices : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager)
        {

            //1. Header
            //2. Payload
            //2.1 Private Claims (ده user الخاصه بال )
            var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.MobilePhone , user.PhoneNumber),
                
            };

            var UserRoles = await userManager.GetRolesAsync(user);

            foreach (var Role in UserRoles)
            {
                AuthClaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            //3. Signature 
            //3.1 Key
            var AuthKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var Token = new JwtSecurityToken(
               issuer: _configuration["JWT:ValidIssuer"],
               audience: _configuration["JWT:ValidAudience"],
               expires: DateTime.Now.AddHours(double.Parse(_configuration["JWT:DurationInDay"])),
               claims: AuthClaims,
               signingCredentials: new SigningCredentials(AuthKey, SecurityAlgorithms.HmacSha256Signature)
               );
            return new JwtSecurityTokenHandler().WriteToken(Token);
        }

        public RefreshToken GenerateRefreshtoken()
        {
            var rondomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(rondomNumber);

            return new RefreshToken()
            {
                Token = Convert.ToBase64String(rondomNumber),
                CreatedOn = DateTime.Now,
                ExpirsesOn = DateTime.Now.AddDays(25),
            };
        }

        
    }
}
