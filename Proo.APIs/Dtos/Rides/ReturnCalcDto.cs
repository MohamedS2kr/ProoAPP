using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Proo.APIs.Dtos.Rides
{
    public class ReturnCalcDto
    {
        public double Price { get; set; }
        public double Distance { get; set; }
        public double Time { get; set; }
    }
}
