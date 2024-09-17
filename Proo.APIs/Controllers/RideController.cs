using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proo.Infrastructer.Data.Context;

namespace Proo.APIs.Controllers
{
    
    public class RideController : BaseApiController
    {
        private readonly ApplicationDbContext _context;

        public RideController(ApplicationDbContext context) 
        {
            _context = context;
        }

        

    }
}
