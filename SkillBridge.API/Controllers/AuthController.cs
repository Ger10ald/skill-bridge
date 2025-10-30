using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Application.Services;
using SkillBridge.Core.Models;
using SkillBridge.Core.Security;
using SkillBridge.Data;

namespace SkillBridge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly SkillBridgeDbContext _db;
        private readonly PasswordCredentialService _creds;
        private readonly IJwtTokenService _jwt;

        public AuthController(
            SkillBridgeDbContext db,
            PasswordCredentialService creds,
            IJwtTokenService jwt)
        {
            _db = db;
            _creds = creds;
            _jwt = jwt;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password are required.");

            var user = await _db.Users.AsNoTracking()
                                      .SingleOrDefaultAsync(u => u.Email == dto.Email.Trim().ToLower(), ct);
            if (user is null) return Unauthorized();

            var result = await _creds.VerifyAsync(user.Id, dto.Password, ct);
            if (!result.Succeeded) return Unauthorized();

            var token = _jwt.Issue(user);
            return Ok(new AuthResult(token.AccessToken, token.ExpiresAtUtc, result.Rotated));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
        {
            if (dto is null ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.UserName) ||
                string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("UserName, Email, and Password are required.");

            var email = dto.Email.Trim().ToLowerInvariant();
            var exists = await _db.Users.AnyAsync(u => u.Email == email, ct);
            if (exists) return Conflict("Email already in use.");

            var user = new User
            {
                UserName = dto.UserName.Trim(),
                Email = email,
                FirstName = dto.FirstName?.Trim(),
                LastName = dto.LastName?.Trim(),
 
                PasswordHash = new byte[] { 1 }, 
                PasswordSalt = new byte[] { 1 }
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct); 

            await _creds.SetOrReplaceAsync(user.Id, dto.Password, ct);

            var token = _jwt.Issue(user);
            return CreatedAtAction(nameof(Login), new { email = user.Email }, new AuthResult(token.AccessToken, token.ExpiresAtUtc, Rotated: false));
        }
    }

    public sealed record LoginDto(string Email, string Password);

    public sealed record RegisterDto(string UserName, string Email, string Password, string? FirstName, string? LastName);

    public sealed record AuthResult(string Token, DateTime ExpiresAtUtc, bool Rotated);
}
