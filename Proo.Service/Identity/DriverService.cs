using Proo.Core.Contract;
using Proo.Core.Contract.Dtos.Identity;
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
    public class DriverService: IDriverService
    {
        private readonly DriverMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public DriverService(IUnitOfWork unitOfWork, PassengerMapper mapper)
        {
            _unitOfWork = unitOfWork;
        }

        public List<DriverDto> GetBy(Expression<Func<Driver, bool>> predicate)
        {
            return _mapper.MapFromSourceToDestination(_unitOfWork.Repositoy<Driver>().GetBy(predicate));
        }

        public int Add(DriverDto driver)
        {
            _unitOfWork.Repositoy<Driver>().Add(_mapper.MapFromDestinationToSource(driver));
            return _unitOfWork.Complete();
        }
    }
}
