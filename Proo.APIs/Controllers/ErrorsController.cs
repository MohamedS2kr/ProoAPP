using Microsoft.AspNetCore.Mvc;
using Proo.Core.Contract.Errors;
#nullable enable
namespace Proo.APIs.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : BaseApiController
    {

        [HttpGet]
        public async Task<IActionResult> NotFound()
        {
            return NotFound(new ApiResponse(404));
        }


    }
}
