namespace Proo.APIs.Dtos.VehicleModel
{
    public class VehicleModelDTO
    {
        public string ModelName { get; set; }
        public int VehicleTypeId { get; set; }
    }
    public class ReturnVehicleModelDTO
    {
        public int Id { get; set; }
        public string ModelName { get; set; }
        public int VehicleTypeId { get; set; }
    }
}
