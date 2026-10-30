using API_proj.Data;
using API_proj.Models;
using API_proj.Models.Dtos;
using API_proj.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API_proj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(RaceDayDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Email == email))
                return Conflict(new { message = "A user with this email already exists." });

            if (dto.DateOfBirth == default || dto.DateOfBirth > DateTime.UtcNow.AddYears(-10))
                return BadRequest(new { message = "User must be at least 10 years old." });

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                Surname = dto.Surname,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // ✅ FIX: auto-assign Participant role
                var participantRole = await _context.Roles
                    .FirstOrDefaultAsync(r => r.RoleName == "Participant");

                if (participantRole == null)
                {
                    // Seed roles if missing (safe fallback for fresh DB)
                    participantRole = new Role
                    {
                        RoleName = "Participant",
                        Description = "Can register for events",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Roles.Add(participantRole);

                    var organiserRole = new Role
                    {
                        RoleName = "Organiser",
                        Description = "Can create and manage events",
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Roles.Add(organiserRole);

                    await _context.SaveChangesAsync();
                }

                _context.Assignments.Add(new Assignment
                {
                    UserId = user.UserId,
                    RoleId = participantRole.RoleId,
                    AssignedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    message = "Database error.",
                    detail = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }

            return Ok(new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                Surname = user.Surname,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            var email = dto.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !user.IsActive ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            var roles = await _context.Assignments
                .Where(a => a.UserId == user.UserId)
                .Join(_context.Roles,
                      a => a.RoleId,
                      r => r.RoleId,
                      (a, r) => r.RoleName)
                .ToListAsync();

            var token = _tokenService.CreateToken(
                user,
                roles.Count > 0 ? roles : new[] { "Participant" }
            );

            return Ok(new { token });
        }

        // GET: api/auth/me
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users
                .Include(u => u.Assignments)
                    .ThenInclude(a => a.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            return Ok(new
            {
                user.UserId,
                user.Email,
                user.FirstName,
                user.Surname,
                user.PhoneNumber,
                user.Address,
                Roles = user.Assignments.Select(a => a.Role!.RoleName)
            });
        }
    }
}