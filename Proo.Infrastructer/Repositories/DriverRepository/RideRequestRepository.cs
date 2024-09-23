using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Repositories.DriverRepository
{
    public class RideRequestRepository : GenaricRepository<RideRequests>
    {
        public RideRequestRepository(ApplicationDbContext dbContext):base(dbContext)
        {
           
        }
    }
}
