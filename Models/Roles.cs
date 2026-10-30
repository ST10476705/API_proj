using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_proj.Models
{
    [Table("ROLES")]
    public class Role
    {
        [Key]
        [Column("ROLESID")]
        public int RoleId { get; set; }

        [Column("ROLENAME")]
        [Required, MaxLength(50)]
        public string RoleName { get; set; } = "";

        [Column("DESCRIPTION")]
        [MaxLength(255)]
        public string? Description { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        // ✅ Navigation: one Role → many Assignments
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}