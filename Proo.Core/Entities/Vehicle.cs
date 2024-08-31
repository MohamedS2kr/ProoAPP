using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string LicensePlate { get; set; }
        public Driver Driver { get; set; }
        public string DriverId { get; set; }
    }
}
