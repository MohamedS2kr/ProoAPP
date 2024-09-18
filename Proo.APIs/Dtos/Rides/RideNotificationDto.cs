namespace Proo.APIs.Dtos.Rides
{
    public class RideNotificationDto
    {
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public string PickupAddress { get; set; }
        public double DropOffLat { get; set; }
        public double DropOffLng { get; set; }
        public string DropOffAddress { get; set; }
        public decimal FarePrice { get; set; }
        public string PassengerId { get; set; }
    }
}
