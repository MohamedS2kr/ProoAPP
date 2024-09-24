using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Repositories.DriverRepository
{
    public class RideRequestRepository : GenaricRepository<RideRequests> , IRideRequestRepository
    {
        public RideRequestRepository(ApplicationDbContext dbContext):base(dbContext)
        {
           
        }

        public Task<RideRequests?> GetActiveTripRequestForCustomer(string PassengerId)
        {
            throw new NotImplementedException();
        }

        public Task<RideRequests?> GetActiveTripRequestForDriver(string driverId)
        {
            throw new NotImplementedException();
        }
    }
}
