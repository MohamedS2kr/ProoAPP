using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Specifications.BidSpecifications
{
    public class BidWithOtherBidsForDriverSpecifications : BaseSpecifications<Bid>
    {
        public BidWithOtherBidsForDriverSpecifications(int rideRequestId , int bidId)
            :base(b => b.RideRequestsId ==rideRequestId && b.Id != bidId)
        {
            
        }
    }
}
