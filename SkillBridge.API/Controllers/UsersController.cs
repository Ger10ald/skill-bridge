using Microsoft.AspNetCore.Mvc;
using SkillBridge.Data;
using SkillBridge.Core.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SkillBridge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SkillBridgeDbContext _context;
        
        public UsersController(SkillBridgeDbContext context)
        {
            _context = context;
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
            
            existingUser.Name = updatedUser.Name;
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
    }
}
