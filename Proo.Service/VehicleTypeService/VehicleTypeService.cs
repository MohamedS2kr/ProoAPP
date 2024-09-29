using Proo.Core.Contract;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.VehicleTypeService
{
    public class VehicleTypeService:IVehicleTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VehicleType>> GetVehicleTypesByCategoryIdAsync(int categoryId)
        {
            var allVehicleTypes = await _unitOfWork.Repositoy<VehicleType>().GetAll();
            var filteredVehicleTypes = allVehicleTypes.Where(vt => vt.CategoryOfVehicleId == categoryId);
            return filteredVehicleTypes;
        }
    }
}
