namespace Proo.Core.Contract.Dtos.VehicleType
{
    public class VehicleTypeDTO
    {
        public string TypeName { get; set; }
        public int CategoryOfVehicleId { get; set; }
        public virtual CategoryOfVehicleDto? CategoryOfVehicle { get; set; }

    }

    

    public class ReturnVehicleTypeDTO
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public int CategoryOfVehicleId { get; set; }
    }
}
