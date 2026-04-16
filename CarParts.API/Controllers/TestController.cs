using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarParts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("test");
        }
    }
}
