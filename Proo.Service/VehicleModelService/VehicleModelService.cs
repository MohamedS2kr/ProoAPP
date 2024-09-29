using Proo.Core.Contract;
using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.VehicleModelService
{
    public class VehicleModelService:IVehicleModelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleModelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VehicleModel>> GetVehicleModelsByVehicleTypeIdAsync(int vehicleTypeId)
        {
            // Fetch all vehicle models
            var allVehicleModels = await _unitOfWork.Repositoy<VehicleModel>().GetAll();

            // Filter the results based on vehicleTypeId
            var filteredVehicleModels = allVehicleModels.Where(vm => vm.VehicleTypeId == vehicleTypeId);

            return filteredVehicleModels;
        }
    }
}
