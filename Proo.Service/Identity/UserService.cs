using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Proo.Core.Contract.Errors;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UserService(UserManager<ApplicationUser> userManager
            , SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public List<ApplicationUser> GetBy(Expression<Func<ApplicationUser, bool>> query)
        {
            return  _userManager.Users.Where(query).ToList();
            
        }

        public async Task<IdentityResult> Update(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> UpdateUserRole(ApplicationUser user,string role)
        {
            return await _userManager.AddToRoleAsync(user,role);
        }

        public async Task<bool> ValidateUserRole(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IList<string>> GetUserRole(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}
