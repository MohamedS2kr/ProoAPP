namespace Proo.APIs.Dtos.Rides
{
    public class ReturnRideRequestDto
    {
        public string ProfilePicture { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Category { get; set; }
        public string PickupAddress { get; set; }
        public string DropOffAddress { get; set; }
        public double Price { get; set; }
        public double Time { get; set; }
        public double Distance { get; set; }
    }
}
