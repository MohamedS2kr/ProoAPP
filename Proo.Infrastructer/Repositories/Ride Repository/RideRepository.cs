using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Repositories.Ride_Repository
{
    public class RideRepository : IRideRepository
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ApplicationDbContext _context;

        public RideRepository(ILoggerFactory loggerFactory , ApplicationDbContext context)
        {
            _loggerFactory = loggerFactory;
            _context = context;
        }


        public async Task<Ride?> GetActiveTripForPassenger(string PassengerId)
        {
                           
            try
            {
                var entity = await _context.Rides.Where(r => r.PassengerId == PassengerId 
                && r.Status < RideStatus.Completed).SingleOrDefaultAsync();  // the passenger is ongoing trip  

                return entity;  // returned the ride for passenger who going the trip 

            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<RideRepository>();
                logger.LogError($"{nameof(GetActiveTripForPassenger)} threw an exception: {ex}");
                throw;
            }
        }   


        public async Task<Ride?> GetActiveTripForDriver(string driverId)
        {
            try
            {
                var entity = await _context.Rides.Where(r => r.DriverId == driverId
                && r.Status < RideStatus.Completed).SingleOrDefaultAsync();  // the driver is ongoing trip 

                return entity;  // the ride not comleted [driver is alaerdy ongoing trip]
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<RideRepository>();
                logger.LogError($"{nameof(GetActiveTripForDriver)} threw an exception: {ex}");

                throw;
            }
        }

 

        public async Task<Ride?> GetTripForPassengerWithPendingPayment(int RideId, string PassengerId)
        {
            try
            {
                var entity = await _context.Rides.Where(r => r.PassengerId == PassengerId
                                            && r.Status == RideStatus.WAITING_FOR_PAYMENT).SingleOrDefaultAsync();

                return entity;
            }
            catch (Exception ex)
            {
                var logger = _loggerFactory.CreateLogger<RideRepository>();
                logger.LogError($"{nameof(GetTripForPassengerWithPendingPayment)} threw an exception: {ex}");

                throw;
            }
        }
    }
}
