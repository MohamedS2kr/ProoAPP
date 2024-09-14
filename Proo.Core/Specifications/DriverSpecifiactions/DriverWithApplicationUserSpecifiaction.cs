using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Specifications.DriverSpecifiactions
{
    public class DriverWithApplicationUserSpecifiaction : BaseSpecifications<Driver>
    {
        public DriverWithApplicationUserSpecifiaction(string userId)
            :base(d => d.UserId == userId)
        {
            Includes.Add(d => d.User);
        }

    }
}
