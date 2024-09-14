using Microsoft.EntityFrameworkCore;
using Proo.Core.Entities;
using Proo.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer
{
    public static class SpecificationEvaluators<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> Inputquery , ISpecifications<T> spec)
        {
            var query = Inputquery; // _context.set<T>()

            if(spec.Criteria is not null)
                query = query.Where(spec.Criteria);   // _context.set<driver>.where(d => d.id == id)

            query = spec.Includes.Aggregate(query, (currentqeury, icludeEpressin) => currentqeury.Include(icludeEpressin));

            return query;
        }
    }
}
