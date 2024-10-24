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
        public double EstimatedDistance { get; set; }
        public double EstimatedTime { get; set;}
        public double FarePrice { get; set; }
        public string PassengerId { get; set; }
        public string Picture { get; set; }
        public decimal rate { get; set; }
        public int NumberOfTrips { get; set; }
        public string Name { get; set; }
    }
}
