using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_proj.Models
{
    [Table("ASSIGNMENT")]                // singular in DB
    public class Assignment
    {
        [Key]
        [Column("ASSIGNMENTID")]
        public int AssignmentId { get; set; }

        [Column("USERID")]
        public int UserId { get; set; }

        [Column("ROLESID")]
        public int RoleId { get; set; }

        [Column("AssignedAt")]
        public DateTime AssignedAt { get; set; }

        // Navigation
        public User? User { get; set; }
        public Role? Role { get; set; }
    }
}