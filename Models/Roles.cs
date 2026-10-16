using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_proj.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RoleName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        [JsonIgnore]
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}