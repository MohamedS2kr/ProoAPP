using Proo.Core.Contract.Dtos.Passenger;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.IdentityInterface
{
    public interface IPassengerService
    {
        List<PassengerDto> GetBy(Expression<Func<Passenger, bool>> predicate);
        int Add(PassengerDto passenger);
    }
}
