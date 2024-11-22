using Proo.Core.Contract;
using Proo.Core.Contract.RideService_Contract;
using Proo.Core.Entities;
using Proo.Infrastructer.Data.Context;
using Proo.Infrastructer.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Infrastructer.Data
{
    public class UnitOfwork : IUnitOfWork 
    {
        private readonly Hashtable _Repository;
        private readonly ApplicationDbContext _context;

        public IRideRequestRepository RideRequestRepository { get ; }

        public IRideRepository RideRepository { get;  }

        public UnitOfwork(ApplicationDbContext context , IRideRequestRepository rideRequestRepository , IRideRepository rideRepository)
        {
            _context = context;
            _Repository = new Hashtable();
            RideRequestRepository = rideRequestRepository;
            RideRepository = rideRepository;    
        }

        public IGenaricRepositoy<T> Repositoy<T>() where T : BaseEntity
        {
            var key = typeof(T).Name;  // driver
            if(!_Repository.ContainsKey(key))
            {
                var repo = new GenaricRepository<T>(_context);
                _Repository.Add(key, repo);
            }

            return _Repository[key] as IGenaricRepositoy<T>;
        }

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();

        public int Complete()
            =>  _context.SaveChanges();

        public async ValueTask DisposeAsync()
            => await _context.DisposeAsync();

      
    }
}
