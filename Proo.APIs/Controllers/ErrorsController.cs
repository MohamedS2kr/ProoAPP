using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proo.APIs.Errors;

namespace Proo.APIs.Controllers
{
  
    public class ErrorsController : BaseApiController
    {

        [HttpGet]
        public async Task<IActionResult> NotFound()
        {
            return NotFound(new ApiResponse(404));
        }


    }
}
