using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SkillBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] 
    public sealed class AdminController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            var sub = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            var name = User.Identity?.Name; 
            var roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToArray();

            return Ok(new { ok = true, sub, name, roles });
        }
    }
}
