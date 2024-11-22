namespace Proo.Core.Contract.Dtos.Rides
{
    public class FindDriversForUserDto
    {
        public string PickupAddress { get; set; }
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        public GenderType DriverGenderSelection { get; set; }
    }
}
