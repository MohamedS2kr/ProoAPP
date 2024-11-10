using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable
namespace Proo.Core.Entities
{
    public class VehicleType:BaseEntity
    {
        public int Id { get; set; }
        public string TypeName { get; set; }  
        public int CategoryOfVehicleId { get; set; }
        public virtual CategoryOfVehicle? CategoryOfVehicle { get; set; }  

        public virtual ICollection<VehicleModel>? vehicleModels { get; set; } 
    }
}
