using Microsoft.AspNetCore.Identity;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Identity.DataSeed
{
    public static class ApplicationIdentityDataSeed
    {
        public static async Task SeedUserAsync(UserManager<ApplicationUser> _userManager)
        {
            var user = new ApplicationUser()
            {
                FirstName = "Mohamed",
                LastName = " Gamal",
                Email = "mohamedGamal@gmail.com",
                UserName = "Mohamed.Gamal",
                PhoneNumber = "01204138365",
            };
            await _userManager.CreateAsync(user, "Mo@@200300");
        }
    }
}
