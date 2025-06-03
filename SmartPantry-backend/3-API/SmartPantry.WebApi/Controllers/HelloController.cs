using Microsoft.AspNetCore.Mvc;

namespace SmartPantry.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Hello, SmartPantry!");
    }
}