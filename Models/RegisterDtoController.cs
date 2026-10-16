using System.ComponentModel.DataAnnotations;

namespace API_proj.Models.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? PhoneNumber { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }
    }
}