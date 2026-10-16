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
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        public EnrolmentsController(RaceDayDbContext db) => _db = db;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // POST: api/events/5/enrolments  body: { "categoriesId": 1 }
        [HttpPost("events/{eventId}/enrolments")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> Enrol(int eventId, [FromBody] Enrolment body)
        {
            var cat = await _db.Categories.FindAsync(body.CategoryId);
            if (cat == null || cat.EventId != eventId)
                return NotFound(new { message = "Category not found for this event" });

            bool exists = await _db.Enrolments.AnyAsync(e =>
                e.UserId == CurrentUserId && e.CategoryId == body.CategoryId);
            if (exists) return Conflict(new { message = "Already enrolled" });

            var enrolment = new Enrolment
            {
                UserId = CurrentUserId,
                EventId = eventId,
                CategoryId = body.CategoryId,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                EnrolledAt = DateTime.Now
            };
            _db.Enrolments.Add(enrolment);
            cat.Participants++;
            await _db.SaveChangesAsync();
            return StatusCode(201, enrolment);
        }

        // GET: api/events/5/enrolments
        [HttpGet("events/{eventId}/enrolments")]
        [Authorize]
        public async Task<IActionResult> GetForEvent(int eventId)
        {
            var list = await _db.Enrolments
                .Include(e => e.User).Include(e => e.Category)
                .Where(e => e.EventId == eventId).ToListAsync();
            return Ok(list);
        }

        // GET: api/enrolments/5
        [HttpGet("enrolments/{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var e = await _db.Enrolments
                .Include(x => x.User).Include(x => x.Category).Include(x => x.Event)
                .FirstOrDefaultAsync(x => x.EnrolmentId == id);
            if (e == null) return NotFound();
            if (e.UserId != CurrentUserId && !User.IsInRole("Organiser")) return Forbid();
            return Ok(e);
        }

        // PUT: api/enrolments/5
        [HttpPut("enrolments/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Enrolment dto)
        {
            var e = await _db.Enrolments.Include(x => x.Event).FirstOrDefaultAsync(x => x.EnrolmentId == id);
            if (e == null) return NotFound();

            bool isOwner = e.UserId == CurrentUserId;
            bool isOrganiser = User.IsInRole("Organiser") && e.Event!.UserId == CurrentUserId;
            if (!isOwner && !isOrganiser) return Forbid();

            e.Status = dto.Status;
            e.PaymentStatus = dto.PaymentStatus;
            await _db.SaveChangesAsync();
            return Ok(e);
        }

        // DELETE: api/enrolments/5
        [HttpDelete("enrolments/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var e = await _db.Enrolments.Include(x => x.Event).FirstOrDefaultAsync(x => x.EnrolmentId == id);
            if (e == null) return NotFound();

            bool isOwner = e.UserId == CurrentUserId;
            bool isOrganiser = User.IsInRole("Organiser") && e.Event!.UserId == CurrentUserId;
            if (!isOwner && !isOrganiser) return Forbid();

            e.Status = "Cancelled";
            await _db.SaveChangesAsync();
            return Ok(new { message = "Enrolment cancelled" });
        }
    }
}