using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Core.Models;
using SkillBridge.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using SkillBridge.Application.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SkillBridge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SkillBridgeDbContext _context;
        private readonly PasswordCredentialService _cred;

        public UsersController(SkillBridgeDbContext context, PasswordCredentialService cred)
        {
            _context = context;
            _cred = cred;
        }
        private static bool TryGetUserId(ClaimsPrincipal user, out int userId)
        {
            userId = 0;
            var sub = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            return int.TryParse(sub, out userId);
        }

        [Authorize]
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordDto dto, CancellationToken ct)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("CurrentPassword and NewPassword are required.");

            if (!TryGetUserId(User, out var userId))
                return Unauthorized(new { reason = "no-or-bad-token" });

            var ok = await _cred.VerifyAsync(userId, dto.CurrentPassword, ct);
            if (!ok.Succeeded) return Unauthorized(new { reason = "verify-failed" }); 

            await _cred.SetOrReplaceAsync(userId, dto.NewPassword, ct);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/password")]
        public async Task<IActionResult> AdminSetPassword(int id, [FromBody] AdminSetPasswordDto dto, CancellationToken ct)
        {
            if (id <= 0) return BadRequest("Invalid user id.");
            if (dto is null || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest("NewPassword is required.");

            var exists = await _context.Users.AsNoTracking().AnyAsync(u => u.Id == id, ct);
            if (!exists) return NotFound();

            await _cred.SetOrReplaceAsync(id, dto.NewPassword, ct);
            return NoContent();
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users); 
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST api/users
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound();
            
            
            existingUser.Email = updatedUser.Email;
            existingUser.Bio = updatedUser.Bio;

            await _context.SaveChangesAsync();
            return NoContent();
               
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user==null)
                return NotFound();
            
            _context.Users.Remove(user);
             await _context.SaveChangesAsync();

            return NoContent();
        }

        public sealed record ChangeMyPasswordDto(string CurrentPassword, string NewPassword);
        public sealed record AdminSetPasswordDto(string NewPassword);
    }
}
