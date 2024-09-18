using Proo.Core.Entities;
using Proo.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract
{
    public interface IGenaricRepositoy<T> where T : BaseEntity
    {
        void Add(T model);

        void Update(T model);

        Task<T?> GetByIdWithSpecAsync(ISpecifications<T> spec);

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
    }
}
