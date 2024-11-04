using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class VehicleModel:BaseEntity
    {
        public int Id { get; set; }
        public string ModelName { get; set; }  
        public int VehicleTypeId { get; set; }
        public virtual VehicleType VehicleType { get; set; }

        // Add navigation property for Vehicles
        public virtual ICollection<Vehicle>? Vehicles { get; set; }
    }
}
