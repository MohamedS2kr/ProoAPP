using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.Passenger_Contract
{
    public interface IPassengerRepository : IGenaricRepositoy<Passenger>
    {
        Task<Passenger?> GetByUserId(string userId);
    }
}
