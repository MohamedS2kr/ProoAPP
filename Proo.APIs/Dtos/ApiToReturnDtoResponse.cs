namespace Proo.APIs.Dtos
{
    public class ApiToReturnDtoResponse
    {
        public DataResponse Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        

        public class DataResponse
        {
            public string Mas { get; set; }
            public int StatusCode { get; set; }
            public List<object> Body { get; set; } = new List<object>();
        }

    }
}
