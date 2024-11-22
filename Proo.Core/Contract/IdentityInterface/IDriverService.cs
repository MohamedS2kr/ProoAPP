using Proo.Core.Contract.Dtos.Identity;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.IdentityInterface
{
    public interface IDriverService
    {
        List<DriverDto> GetBy(Expression<Func<Driver, bool>> predicate);
        int Add(DriverDto driver);
    }
}
