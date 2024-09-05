using Proo.Core.Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Repositories
{
    public class GenaricRepository<T> : IGenaricRepositoy<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _context;

        public GenaricRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void  Add(T model)
            => _context.Set<T>().Add(model);
           
    }
}
