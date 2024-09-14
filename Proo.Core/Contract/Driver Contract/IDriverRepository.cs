using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.Driver_Contract
{
    public interface IDriverRepository : IGenaricRepositoy<Driver>
    {
        Task<Driver?> getByUserId(string userId);
    }
}
