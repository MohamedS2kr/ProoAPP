using Microsoft.AspNetCore.Identity;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.IdentityInterface
{
    public interface IUserService
    {
        List<ApplicationUser> GetBy(Expression<Func<ApplicationUser, bool>> query);
        Task<IdentityResult> Update(ApplicationUser user);
        Task<IdentityResult> UpdateUserRole(ApplicationUser user, string role);
        Task<bool> ValidateUserRole(ApplicationUser user, string role);
        Task<IList<string>> GetUserRole(ApplicationUser user);
    }
}
