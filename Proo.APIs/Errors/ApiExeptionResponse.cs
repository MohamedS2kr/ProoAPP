namespace Proo.APIs.Errors
{
    public class ApiExeptionResponse : ApiResponse
    {
        public string? Details { get; set; }

        public ApiExeptionResponse(int _statuscode , string? _message = null , string? _details = null)
            :base(_statuscode , _message)
        {
            Details = _details;
        }

    }
}
