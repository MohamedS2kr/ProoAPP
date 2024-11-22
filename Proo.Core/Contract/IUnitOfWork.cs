using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenaricRepositoy<T> Repositoy<T>() where T : BaseEntity;

        Task<int> CompleteAsync();
        int Complete();
        public IRideRequestRepository RideRequestRepository { get;  }
        public IRideRepository RideRepository { get;  }


    }
}
