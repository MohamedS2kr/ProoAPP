using Microsoft.EntityFrameworkCore;
using Proo.Core.Contract.Driver_Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Repositories.DriverRepository
{
    public class DriverRepository : GenaricRepository<Driver> , IDriverRepository
    {
        public DriverRepository(ApplicationDbContext context)
            :base(context)
        {
            
        }

        public async Task<Driver?> getByUserId(string userId)
            => await _context.Drivers.Where(d => d.UserId == userId).AsNoTracking().FirstOrDefaultAsync();
    }
}
