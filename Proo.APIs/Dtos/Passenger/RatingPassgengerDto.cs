using System.ComponentModel.DataAnnotations;

namespace Proo.APIs.Dtos.Passenger
{
    public class RatingPassgengerDto
    {
        [Range(0,5)]
        public int Score { get; set; } // Rating from 1 to 5 [rang dataAnnotation]
        public string? Review { get; set; }
    }
}
