using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class VehicleType:BaseEntity
    {
        public int Id { get; set; }
        public string TypeName { get; set; }  
        public int CategoryOfVehicleId { get; set; }
        public CategoryOfVehicle? CategoryOfVehicle { get; set; }  

        public ICollection<VehicleModel>? vehicleModels { get; set; } 
    }
}
