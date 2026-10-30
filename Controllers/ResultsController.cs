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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        public ResultsController(RaceDayDbContext db) => _db = db;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // GET: api/events/5/results
        [HttpGet("events/{eventId}/results")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForEvent(int eventId)
        {
            var results = await _db.Results
                .Include(r => r.Enrolment).ThenInclude(e => e!.User)
                .Where(r => r.Enrolment!.Category!.EventId == eventId)
                .ToListAsync();
            return Ok(results);
        }

        // POST: api/results
        [HttpPost("results")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Create([FromBody] Result result)
        {
            var enrolment = await _db.Enrolments
                .Include(e => e.Category).ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(e => e.EnrolmentId == result.EnrolmentId);
            if (enrolment == null) return NotFound();
            if (enrolment.Category!.Event!.UserId != CurrentUserId) return Forbid();

            result.CategoryId = enrolment.CategoryId;   // ✅ Ensure set
            result.Status = "Pending";
            result.CreatedAt = DateTime.Now;
            result.UpdatedAt = DateTime.Now;

            _db.Results.Add(result);
            await _db.SaveChangesAsync();
            return StatusCode(201, result);
        }

        // PUT: api/results/5
        [HttpPut("results/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Update(int id, [FromBody] Result dto)
        {
            var r = await _db.Results
                .Include(x => x.Enrolment).ThenInclude(e => e!.Category).ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(x => x.ResultsId == id);
            if (r == null) return NotFound();
            if (r.Enrolment!.Category!.Event!.UserId != CurrentUserId) return Forbid();

            r.CategoryId = r.Enrolment.CategoryId;   // ✅ FIX: keep it consistent
            r.FinishTime = dto.FinishTime;
            r.OverallRank = dto.OverallRank;
            r.CategoryRank = dto.CategoryRank;
            r.Notes = dto.Notes;
            r.Status = dto.Status;
            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return Ok(r);
        }

        // PUT: api/results/5/verify
        [HttpPut("results/{id}/verify")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Verify(int id)
        {
            var r = await _db.Results
                .Include(x => x.Enrolment).ThenInclude(e => e!.Category).ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(x => x.ResultsId == id);
            if (r == null) return NotFound();
            if (r.Enrolment!.Category!.Event!.UserId != CurrentUserId) return Forbid();

            r.Status = "Verified";
            r.VerifiedBy = CurrentUserId;
            r.VerifiedAt = DateTime.Now;
            r.UpdatedAt = DateTime.Now;
            await _db.SaveChangesAsync();
            return Ok(r);
        }

        // DELETE: api/results/5
        [HttpDelete("results/{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.Results
                .Include(x => x.Enrolment).ThenInclude(e => e!.Category).ThenInclude(c => c!.Event)
                .FirstOrDefaultAsync(x => x.ResultsId == id);
            if (r == null) return NotFound();
            if (r.Enrolment!.Category!.Event!.UserId != CurrentUserId) return Forbid();

            _db.Results.Remove(r);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Result deleted" });
        }
    }
}