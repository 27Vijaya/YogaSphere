using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace YogaSphere.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                application = "YogaSphere API",
                timestamp = DateTime.UtcNow

            });
        }
    }
}
    