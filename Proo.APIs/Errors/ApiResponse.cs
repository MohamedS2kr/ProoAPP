
namespace Proo.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }

        public ApiResponse(int _statusCode , string? _message = null)
        {
            StatusCode = _statusCode;
            Message = _message ?? ApplyMessageByStatusCode(_statusCode);
        }

        private string? ApplyMessageByStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Reuest , You have made",
                401 => "UnAuthorize , you are not",
                404 => "Not Found",
                500 => "Error are the path to dark side , internal server error",
                _ => null

            };
        }
    }
}
