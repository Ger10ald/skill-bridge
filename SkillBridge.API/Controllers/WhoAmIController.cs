using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SkillBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class WhoAmIController : ControllerBase
    {
        [HttpGet]
        [Authorize] 
        public IActionResult Get()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var name = User.Identity?.Name; 
            var email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToArray();

            return Ok(new { userId, name, email, roles });
        }
    }
}
