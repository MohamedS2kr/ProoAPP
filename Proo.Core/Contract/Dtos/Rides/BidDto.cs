using Proo.Core.Entities;

namespace Proo.Core.Contract.Dtos.Rides
{
    public class BidDto
    {

        public decimal OfferedPrice { get; set; }

        public int Eta { get; set; }

        public string DriverId { get; set; }

        public int RideRequestsId { get; set; }
    }
}
