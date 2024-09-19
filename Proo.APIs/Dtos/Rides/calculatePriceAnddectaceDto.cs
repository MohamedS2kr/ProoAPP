using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Rides
{
    public class calculatePriceAnddectaceDto
    {
        [Required]
        public string Category { get; set; }
        [Required]
        public double PickUpLat { get; set; }
        [Required]
        public double PickUpLon { get; set; }
        [Required]
        public double DroppOffLat { get; set; }
        [Required]
        public double DroppOffLon { get; set; }
    }
}
