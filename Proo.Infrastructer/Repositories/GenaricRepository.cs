using Microsoft.EntityFrameworkCore;
using Proo.Core.Contract;
using Proo.Core.Entities;
using Proo.Core.Specifications;
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
        private protected readonly ApplicationDbContext _context;

        public GenaricRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public void  Add(T model)
            => _context.Set<T>().Add(model);

        public void Delete(T model)
            => _context.Set<T>().Remove(model);

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
            => await _context.Set<T>().FindAsync(id);

        public async Task<T?> GetByIdWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }


        public void Update(T model)
            => _context.Set<T>().Update(model);


        private IQueryable<T> ApplySpecification(ISpecifications<T> spec)
        {
            return SpecificationEvaluators<T>.GetQuery(_context.Set<T>(), spec);
        }


    }
}
