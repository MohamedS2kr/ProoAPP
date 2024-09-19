namespace Proo.APIs.Dtos
{
    public class ApiToReturnDtoResponse
    {
        public DataResponse Data { get; set; }
        
        public class DataResponse
        {
            public string Mas { get; set; }
            public int StatusCode { get; set; }
            public object Body { get; set; } 
        }

    }
}
