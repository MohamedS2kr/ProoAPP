namespace Proo.APIs.Dtos.Rides
{
    public class RideRequestDto
    {   
        public string PickupAddress { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }

        public string DropOffAddress { get; set; }
        public double DropoffLatitude { get; set; }
        public double DropoffLongitude { get; set; }

        public string Category { get; set; }
        public decimal FarePrice { get; set; }
    }
}
