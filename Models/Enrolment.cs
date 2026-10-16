using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_proj.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [Required]
        public int EventId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(EventId))]
        public Event? Event { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(CategoryId))]
        public Category? Category { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        [MaxLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid";

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public Result? Result { get; set; }
    }
}