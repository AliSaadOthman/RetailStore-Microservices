using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("This is a protected resource.");
        }

        [HttpGet("profile")]
        [Authorize(Policy = "CustomerPolicy")]
        public IActionResult GetProfile()
        {
            return Ok("This is a protected customer resource.");
        }
    }

}
