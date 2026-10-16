using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_proj.Data;
using API_proj.Models;

namespace API_proj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public RoleController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/roles?search=admin&sortBy=roleName&sortDir=asc
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Role>>> GetRoles(
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "roleId",
            [FromQuery] string? sortDir = "asc")
        {
            var query = _context.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(r => r.RoleName.Contains(search));

            var descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            query = (sortBy?.ToLower()) switch
            {
                "rolename" => descending ? query.OrderByDescending(r => r.RoleName) : query.OrderBy(r => r.RoleName),
                _ => descending ? query.OrderByDescending(r => r.RoleId) : query.OrderBy(r => r.RoleId),
            };

            return await query.ToListAsync();
        }

        // GET: api/roles/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            if (id <= 0) return BadRequest(new { error = "Id must be greater than 0." });

            var role = await _context.Roles.FindAsync(id);

            if (role == null) return NotFound(new { error = $"Role with id {id} was not found." });

            return role;
        }

        // POST: api/roles
        [HttpPost]
        public async Task<ActionResult<Role>> CreateRole([FromBody] Role role)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRole), new { id = role.RoleId }, role);
        }

        // PUT: api/roles/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] Role role)
        {
            if (id <= 0) return BadRequest(new { error = "Id must be greater than 0." });
            if (id != role.RoleId) return BadRequest(new { error = "Route id does not match body RoleId." });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _context.Roles.AnyAsync(r => r.RoleId == id);
            if (!exists) return NotFound(new { error = $"Role with id {id} was not found." });

            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(role);
        }

        // DELETE: api/roles/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            if (id <= 0) return BadRequest(new { error = "Id must be greater than 0." });

            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound(new { error = $"Role with id {id} was not found." });

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}