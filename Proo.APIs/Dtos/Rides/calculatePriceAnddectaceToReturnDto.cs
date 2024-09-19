namespace Proo.APIs.Dtos.Rides
{
    public class calculatePriceAnddectaceToReturnDto
    {
        public string Category { get; set; } = null!;
        public double PickUpLat { get; set; }
        public double PickUpLon { get; set; }
        public double DroppOffLat { get; set; }
        public double DroppOffLon { get; set; }
        public decimal Price { get; set; }
        public decimal Destance { get; set; }
        public int Time { get; set; }
    }
}
