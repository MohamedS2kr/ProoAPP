using System.ComponentModel.DataAnnotations;

namespace Proo.Core.Contract.Dtos.Driver
{
    public class DriverLocationDto
    {
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
    }
}
