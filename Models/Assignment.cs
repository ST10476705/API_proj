using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_proj.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [Required]
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public Role? Role { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    }
}