using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_proj.Models
{
    public class Category
    {
        [Key]
        public int CategoriesId { get; set; }

        [Required]
        [MaxLength(150)]
        public string CategoryName { get; set; } = string.Empty;

        public double Distance { get; set; }

        [MaxLength(50)]
        public string? AgeGroup { get; set; }

        [MaxLength(20)]
        public string? GenderRestriction { get; set; }

        public int MaxParticipants { get; set; }

        public decimal Fee { get; set; }

        public int Participants { get; set; } = 0;

        [Required]
        public int EventId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}