using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Bid : BaseEntity
    {
        public int Id { get; set; }

        public decimal OfferedPrice { get; set; }
        public DateTime CreatedAt { get; set; }

        public int Eta { get; set; }

        public BidStatus BidStatus { get; set; }

        public string DriverId { get; set; }
        public Driver Driver { get; set; }

        public int RideRequestsId { get; set; }
        public RideRequests? Ride { get; set; }
    }

    public enum BidStatus
    {
        Pendding ,
        Accepted,
        Rejected
    }
}
