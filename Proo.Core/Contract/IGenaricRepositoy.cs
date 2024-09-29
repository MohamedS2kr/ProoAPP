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
        void Delete(T model);
        Task<IReadOnlyList<T>> GetAll();
        
        Task<T?> GetByIdAsync(int id);

        Task<T?> GetDriverOrPassengerByIdAsync(string Id);
        Task<T?> GetByIdWithSpecAsync(ISpecifications<T> spec);

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
    }
}
