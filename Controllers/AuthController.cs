using API_proj.Data;
using API_proj.Models;
using API_proj.Models.Dtos;
using API_proj.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required." });

            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _context.Users.AnyAsync(u => u.Email == email))
                return Conflict(new { message = "A user with this email already exists." });

            var user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                Surname = dto.Surname,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                _context.Users.Add(user);
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

            var userId = user.UserId.ToString();

            var roles = await _context.Assignments
                .Where(a => a.UserId.ToString() == userId)
                .Join(_context.Roles,
                      a => a.RoleId.ToString(),
                      r => r.RoleId.ToString(),
                      (a, r) => r.RoleName)
                .ToListAsync();

            // Your TokenService.CreateToken takes a single role string
            var token = _tokenService.CreateToken(user, roles.FirstOrDefault() ?? "Participant");
            return Ok(new { token });
        }
    }
}