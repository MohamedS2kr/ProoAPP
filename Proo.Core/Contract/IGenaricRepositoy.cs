using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract
{
    public interface IGenaricRepositoy<T> where T : BaseEntity
    {
        Task<int> Add(T model);
    }
}
