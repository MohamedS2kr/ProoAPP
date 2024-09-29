using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Service.VehicleModelService
{
    public interface IVehicleModelService
    {
        Task<IEnumerable<VehicleModel>> GetVehicleModelsByVehicleTypeIdAsync(int vehicleTypeId);
    }
}
