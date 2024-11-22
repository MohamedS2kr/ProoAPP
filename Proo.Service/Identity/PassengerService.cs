using Proo.Core.Contract;
using Proo.Core.Contract.Dtos.Passenger;
using Proo.Core.Contract.IdentityInterface;
using Proo.Core.Entities;
using Proo.Service.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.Identity
{
    public class PassengerService: IPassengerService
    {
        private readonly PassengerMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public PassengerService(IUnitOfWork unitOfWork, PassengerMapper mapper) { 
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public List<PassengerDto> GetBy(Expression<Func<Passenger, bool>> predicate)
        {
            return _mapper.MapFromSourceToDestination(_unitOfWork.Repositoy<Passenger>().GetBy(predicate));
        }

        public int Add(PassengerDto passenger) { 
            _unitOfWork.Repositoy<Passenger>().Add(_mapper.MapFromDestinationToSource(passenger));
            return _unitOfWork.Complete();
        }

    }
}
