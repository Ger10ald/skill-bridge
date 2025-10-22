using Microsoft.AspNetCore.Mvc;

namespace SkillBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase   
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { status = "SkillBridge OK", timestamp = DateTime.UtcNow });
        }
    }
}
