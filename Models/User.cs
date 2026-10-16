using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_proj.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [JsonIgnore] // never echo the password hash back to the client
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Surname { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? PhoneNumber { get; set; }

        [MaxLength(300)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties — all ignored on serialization to prevent loops
        [JsonIgnore]
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

        [JsonIgnore]
        public ICollection<Event> Events { get; set; } = new List<Event>();

        [JsonIgnore]
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}