using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_proj.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        [Column("USERID")]
        public int UserId { get; set; }

        [Column("EMAIL")]
        [Required, MaxLength(100)]
        public string Email { get; set; } = "";

        [Column("FIRSTNAME")]
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = "";

        [Column("SURNAME")]
        [Required, MaxLength(50)]
        public string Surname { get; set; } = "";

        [Column("GENDER")]
        [MaxLength(20)]
        public string? Gender { get; set; }

        [Column("PHONENUMBER")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Column("ADRESS")]                  // DB typo kept as-is
        [MaxLength(255)]
        public string? Address { get; set; }

        [Column("DATEOFBIHTH")]             // DB typo kept as-is
        public DateTime DateOfBirth { get; set; }

        [Column("PasswordHash")]
        [Required, MaxLength(255)]
        public string PasswordHash { get; set; } = "";

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        // ✅ Navigation: one User → many Assignments
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}