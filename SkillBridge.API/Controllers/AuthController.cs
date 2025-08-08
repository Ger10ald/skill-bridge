using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Data;
using SkillBridge.Core.Models;
using SkillBridge.Application.Dtos;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SkillBridge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SkillBridgeDbContext _context;
    
        public AuthController(SkillBridgeDbContext context)
        {
            _context = context;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto request)
        {
            if (await UserExists(request.Email))
                return BadRequest("User already exists.");

            using var hmac = new HMACSHA512();

            var user = new User
            {
                Email = request.Email,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password)),
                Bio = request.Bio
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { user.Id, user.Email });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid credentials.");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid credentials.");
            }

            return Ok(new { message = "Login successful", user.Id, user.Email });
        }

        private async Task<bool> UserExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email.ToLower());
        }
    }
}
