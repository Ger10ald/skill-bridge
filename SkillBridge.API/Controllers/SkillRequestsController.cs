using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillBridge.Core.Models;
using SkillBridge.Data;

namespace SkillBridge.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillRequestsController : ControllerBase
    {
        private readonly SkillBridgeDbContext _context;

        public SkillRequestsController(SkillBridgeDbContext context)
        {
            _context = context;
        }

        // GET: api/SkillRequests
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillRequest>>> GetSkillRequests()
        {
            return await _context.SkillRequests.ToListAsync();
        }

        // GET: api/SkillRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SkillRequest>> GetSkillRequest(int id)
        {
            var skillRequest = await _context.SkillRequests.FindAsync(id);

            if (skillRequest == null)
            {
                return NotFound();
            }

            return skillRequest;
        }

        // PUT: api/SkillRequests/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkillRequest(int id, SkillRequest skillRequest)
        {
            if (id != skillRequest.Id)
            {
                return BadRequest();
            }

            _context.Entry(skillRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SkillRequestExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SkillRequests
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SkillRequest>> PostSkillRequest(SkillRequest skillRequest)
        {
            _context.SkillRequests.Add(skillRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSkillRequest", new { id = skillRequest.Id }, skillRequest);
        }

        // DELETE: api/SkillRequests/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkillRequest(int id)
        {
            var skillRequest = await _context.SkillRequests.FindAsync(id);
            if (skillRequest == null)
            {
                return NotFound();
            }

            _context.SkillRequests.Remove(skillRequest);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SkillRequestExists(int id)
        {
            return _context.SkillRequests.Any(e => e.Id == id);
        }
    }
}
