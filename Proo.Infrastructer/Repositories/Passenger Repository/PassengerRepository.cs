using Microsoft.EntityFrameworkCore;
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Contract.Passenger_Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Repositories.Passenger_Repository
{
    public class PassengerRepository : GenaricRepository<Passenger>, IPassengerRepository
    {
        public PassengerRepository(ApplicationDbContext context)
            : base(context)
        {
        }

        public Task GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Passenger?> GetByUserId(string userId)
            => await _context.Passengers.Where(p => p.UserId == userId).AsNoTracking().FirstOrDefaultAsync();
    }
}
