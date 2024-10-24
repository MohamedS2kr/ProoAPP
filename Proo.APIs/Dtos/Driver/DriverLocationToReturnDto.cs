using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Driver
{
    public class DriverLocationToReturnDto
    {
        public string DriverId { get; set; }
        public double Latitude { get; set; }
    
        public double Longitude { get; set; }
    }
}
