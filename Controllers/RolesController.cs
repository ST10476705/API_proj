using API_proj.Data;
using API_proj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_proj.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        public RolesController(RaceDayDbContext db) => _db = db;

        // GET: api/roles
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
            => Ok(await _db.Roles.ToListAsync());

        // GET: api/roles/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            return role == null ? NotFound() : Ok(role);
        }

        // POST: api/roles
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create([FromBody] Role role)
        {
            if (string.IsNullOrWhiteSpace(role.RoleName))
                return BadRequest(new { message = "RoleName is required" });

            if (await _db.Roles.AnyAsync(r => r.RoleName == role.RoleName))
                return Conflict(new { message = "Role already exists" });

            role.CreatedAt = DateTime.Now;
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
            return StatusCode(201, role);
        }

        // PUT: api/roles/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] Role dto)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();

            role.RoleName = dto.RoleName;
            role.Description = dto.Description;
            await _db.SaveChangesAsync();
            return Ok(role);
        }

        // DELETE: api/roles/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _db.Roles.FindAsync(id);
            if (role == null) return NotFound();

            bool inUse = await _db.Assignments.AnyAsync(a => a.RoleId == id);
            if (inUse) return BadRequest(new { message = "Cannot delete role — it is assigned to users" });

            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Role deleted" });
        }
    }
}