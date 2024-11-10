namespace Proo.APIs.Dtos.CategoryOfVehicle
{
    public class CategoryDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public IFormFile ImageOfCategory { get; set; }


    }
    public class ReturnCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageOfCategory { get; set; }


    }
}
