using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Driver
{
    public class DriverLocationDto
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}
