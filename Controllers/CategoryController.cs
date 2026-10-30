using API_proj.Data;
using API_proj.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_proj.Controllers
{
    [ApiController]
    [Route("api")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        public CategoriesController(RaceDayDbContext db) => _db = db;

        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/events/5/categories
        [HttpGet("events/{eventId}/categories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForEvent(int eventId)
        {
            var cats = await _db.Categories
                .Where(c => c.EventId == eventId)
                .ToListAsync();
            return Ok(cats);
        }

        // GET: api/categories/5
        [HttpGet("categories/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            var c = await _db.Categories
                .Include(x => x.Enrolments)
                .FirstOrDefaultAsync(x => x.CategoriesId == id);
            return c == null ? NotFound() : Ok(c);
        }

        // POST: api/categories  (Organiser only)
        [HttpPost("categories")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create([FromBody] Category cat)
        {
            var ev = await _db.Events.FindAsync(cat.EventId);
            if (ev == null)
                return NotFound(new { message = "Event not found" });

            if (ev.UserId != CurrentUserId) return Forbid();   // ✅ FIX

            cat.CreatedAt = DateTime.Now;
            _db.Categories.Add(cat);
            await _db.SaveChangesAsync();
            return StatusCode(201, cat);
        }

        // PUT: api/categories/5
        [HttpPut("categories/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] Category dto)
        {
            var cat = await _db.Categories
                .Include(c => c.Event)
                .FirstOrDefaultAsync(c => c.CategoriesId == id);

            if (cat == null) return NotFound();
            if (cat.Event!.UserId != CurrentUserId) return Forbid();

            cat.CategoryName = dto.CategoryName;
            cat.Distance = dto.Distance;
            cat.AgeGroup = dto.AgeGroup;
            cat.GenderRestriction = dto.GenderRestriction;
            cat.MaxParticipants = dto.MaxParticipants;
            cat.Fee = dto.Fee;

            await _db.SaveChangesAsync();
            return Ok(cat);
        }
    }
}