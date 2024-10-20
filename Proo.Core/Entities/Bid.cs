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

        public BidStatus BidStatus { get; set; } = BidStatus.Pending;

        public string DriverId { get; set; }
        public virtual Driver Driver { get; set; }

        public int RideRequestsId { get; set; }
        public virtual RideRequests? Ride { get; set; }
    }

    public enum BidStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
