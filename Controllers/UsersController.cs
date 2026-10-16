using API_proj.Data;
using API_proj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_proj.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        public UsersController(RaceDayDbContext db) => _db = db;

        // GET: api/users
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _db.Users
                .Select(u => new { u.UserId, u.Email, u.FirstName, u.Surname, u.IsActive })
                .ToListAsync();
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var me = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (me != id && !User.IsInRole("Organiser")) return Forbid();

            var user = await _db.Users.FindAsync(id);
            return user == null ? NotFound() : Ok(new { user.UserId, user.Email, user.FirstName, user.Surname });
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] User dto)
        {
            var me = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (me != id) return Forbid();

            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FirstName = dto.FirstName;
            user.Surname = dto.Surname;
            user.PhoneNumber = dto.PhoneNumber;
            user.Address = dto.Address;
            user.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return Ok(user);
        }

        // PUT: api/users/5/role?roleName=Organiser
        [HttpPut("{id}/role")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateRole(int id, [FromQuery] string roleName)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            var role = await _db.Roles.FirstOrDefaultAsync(r => r.RoleName == roleName);
            if (role == null) return BadRequest(new { message = "Invalid role" });

            var existing = _db.Assignments.Where(a => a.UserId == id);
            _db.Assignments.RemoveRange(existing);

            _db.Assignments.Add(new Assignment { UserId = id, RoleId = role.RoleId });
            await _db.SaveChangesAsync();
            return Ok(new { message = "User role updated", roleName });
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "User deactivated" });
        }
    }
}