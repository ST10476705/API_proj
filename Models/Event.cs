using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_proj.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string Venue { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Location { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int UserId { get; set; }

        public int MaxParticipants { get; set; } = 100;

        [Column(TypeName = "decimal(10,2)")]
        public decimal RegistrationFee { get; set; } = 0.00m;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft";

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [JsonIgnore]
        public ICollection<Category> Categories { get; set; } = new List<Category>();

        [JsonIgnore]
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}