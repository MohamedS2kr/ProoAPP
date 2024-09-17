using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proo.Core.Entities
{
    public class Payment : BaseEntity
    {
        public string Id { get; set; }
        public string RideId { get; set; }
        public Ride Ride { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
    }


    public enum PaymentMethod
    {
        Cash
    }

    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed
    }

}
