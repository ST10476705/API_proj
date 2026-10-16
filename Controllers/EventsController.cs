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
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _db;

        public EventsController(RaceDayDbContext db) => _db = db;

        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private static readonly string[] AllowedStatuses =
            { "Draft", "Published", "Open", "Closed", "Cancelled", "Completed" };

        // POST: api/events
        [HttpPost("events")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] API_proj.Models.Event dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.EventDate <= DateTime.UtcNow)
                return BadRequest(new { error = "EventDate must be in the future." });

            var status = string.IsNullOrWhiteSpace(dto.Status) ? "Draft" : dto.Status;
            if (!AllowedStatuses.Contains(status))
                return BadRequest(new { error = $"Status must be one of: {string.Join(", ", AllowedStatuses)}" });

            var entity = new API_proj.Models.Event
            {
                EventName = dto.EventName,
                EventDate = dto.EventDate,
                Venue = dto.Venue,
                Location = dto.Location,
                Description = dto.Description,
                MaxParticipants = dto.MaxParticipants == 0 ? 100 : dto.MaxParticipants,
                RegistrationFee = dto.RegistrationFee,
                Status = status,
                UserId = CurrentUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Events.Add(entity);
            await _db.SaveChangesAsync();

            return StatusCode(201, entity);
        }

        // GET: api/events
        [HttpGet("events")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.Events
                .Include(e => e.Categories)
                .ToListAsync();

            return Ok(events);
        }

        // GET: api/events/5
        [HttpGet("events/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _db.Events
                .Include(e => e.Categories)
                .FirstOrDefaultAsync(e => e.EventId == id);

            return ev == null ? NotFound() : Ok(ev);
        }

        // PUT: api/events/5
        [HttpPut("events/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] API_proj.Models.Event dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _db.Events.FindAsync(id);
            if (existing == null) return NotFound();
            if (existing.UserId != CurrentUserId) return Forbid();

            if (dto.EventDate <= DateTime.UtcNow)
                return BadRequest(new { error = "EventDate must be in the future." });

            if (!AllowedStatuses.Contains(dto.Status))
                return BadRequest(new { error = $"Status must be one of: {string.Join(", ", AllowedStatuses)}" });

            existing.EventName = dto.EventName;
            existing.EventDate = dto.EventDate;
            existing.Venue = dto.Venue;
            existing.Location = dto.Location;
            existing.Description = dto.Description;
            existing.MaxParticipants = dto.MaxParticipants;
            existing.RegistrationFee = dto.RegistrationFee;
            existing.Status = dto.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(existing);
        }
    }
}