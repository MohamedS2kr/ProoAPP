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
                FullName = "mohamed gamal",
                Email = "mohamedGamal@gmail.com",
                UserName = "Mohamed.Gamal",
                PhoneNumber = "01204138365",
            };
            await _userManager.CreateAsync(user, "Mo@@200300");
        }
        public static async Task SeedRoleForUserAsync(RoleManager<IdentityRole> _roleManager)
        {

            if (_roleManager.Roles.Count() == 0)
            {
                var roles = new List<IdentityRole>
                {
                    new IdentityRole{Name = "passenger"},
                    new IdentityRole{Name = "driver"}
                };
            
                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(role);
                } 
            }
        }
    }
}
