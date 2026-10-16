using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_proj.Models
{
    public class Result
    {
        [Key]
        public int ResultsId { get; set; }

        [Required]
        public int EnrolmentsId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(EnrolmentsId))]
        public Enrolment? Enrolment { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? OverallRank { get; set; }

        public int? CategoryRank { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public bool IsVerified { get; set; } = false;

        public DateTime? VerifiedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}