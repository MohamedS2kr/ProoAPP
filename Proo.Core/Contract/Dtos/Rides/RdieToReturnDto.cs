using Proo.Core.Entities;
using System.Runtime.Serialization;

namespace Proo.Core.Contract.Dtos.Rides
{
    public class RdieToReturnDto
    {
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public string PickupAddress { get; set; }
        public double DropOffLat { get; set; }
        public double DropOffLng { get; set; }
        public string DropOffAddress { get; set; }
        public decimal FarePrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PassengerId { get; set; }
        public string DriverId { get; set; }
       
        public string Status { get; set; }
    }
}
