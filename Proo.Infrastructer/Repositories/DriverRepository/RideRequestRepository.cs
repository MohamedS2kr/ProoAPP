using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILoggerFactory _loggerFactory;

        public RideRequestRepository(ApplicationDbContext dbContext , ILoggerFactory loggerFactory)
            :base(dbContext)
        {
            _loggerFactory = loggerFactory;
        }

        public async Task<RideRequests?> GetActiveTripRequestForCustomer(string PassengerId)
        {
            // Conditions for active trip_request request: -
            // 1. driver is found, but trip_request not started yet!
            // 2. driver is not found yet, but trip_request request is last updated is less than one minute ago!

            try
            {
                var OneMinuteAgo = DateTime.Now.AddMinutes(-1);

                var tripReuest =  await _context.Set<RideRequests>().Where(r => r.PassengerId == PassengerId &&
                (r.Status > RideRequestStatus.NO_DRIVER_FOUND && r.Status < RideRequestStatus.TRIP_STARTED) ||
                (r.Status == RideRequestStatus.NO_DRIVER_FOUND && r.LastModifiedAt < OneMinuteAgo )).SingleOrDefaultAsync();

                return tripReuest;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<RideRequestRepository>();
                logger.LogError($"{nameof(GetActiveTripRequestForCustomer)} threw an exception: {ex}");
                throw;
            }
        }

        public async Task<RideRequests?> GetActiveTripRequestForDriver(string driverId)
        {
            try
            {
                var triprequest = await _context.Set<RideRequests>().Where(r => r.DriverId == driverId
                && (r.Status > RideRequestStatus.NO_DRIVER_FOUND && r.Status < RideRequestStatus.TRIP_STARTED)).SingleOrDefaultAsync();

                return triprequest;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<RideRequestRepository>();
                logger.LogError($"{nameof(GetActiveTripRequestForDriver)} threw an exception: {ex}");
                throw;
            }
        }
    }
}
