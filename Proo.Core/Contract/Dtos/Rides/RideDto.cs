using Proo.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Contract.Dtos.Rides
{
    public class RideDto
    {
        public int Id { get; set; }
        public int RideRequestsId { get; set; }
        public virtual RideRequestDto? RideRequests { get; set; }
        public string PassengerId { get; set; } // ID of the passenger
        public string DriverId { get; set; } //  ID of the driver who accepted the request

        public LocationsDto PickupLocation { get; set; }
        public LocationsDto DestinationLocation { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastModifiedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public decimal FarePrice { get; set; }
        public RideStatus Status { get; set; }
        public PaymentMethod paymentMethod { get; set; }
    }
}
