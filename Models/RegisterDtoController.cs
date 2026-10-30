using System;
using System.ComponentModel.DataAnnotations;

namespace API_proj.Models.Dtos
{
    public class RegisterDto
    {
        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = "";

        [Required, MinLength(6), MaxLength(100)]
        public string Password { get; set; } = "";

        [Required, MaxLength(50)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(50)]
        public string Surname { get; set; } = "";

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        // Required by the USERS.DATEOFBIHTH column (NOT NULL)
        // and the DB CHECK that user must be >= 10 years old.
        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}