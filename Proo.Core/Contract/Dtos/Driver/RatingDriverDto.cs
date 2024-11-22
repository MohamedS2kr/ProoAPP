using System.ComponentModel.DataAnnotations;

namespace Proo.Core.Contract.Dtos.Driver
{
    public class RatingDriverDto
    {
        [Range(0, 5)]
        public int Score { get; set; } // Rating from 1 to 5 [rang dataAnnotation]
        public string? Review { get; set; }
    }
}
