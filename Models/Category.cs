using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_proj.Models
{
    [Table("CATEGORIES")]
    public class Category
    {
        [Key]
        [Column("CATEGORIESID")]
        public int CategoriesId { get; set; }

        [Column("EVENTSDID")]
        public int EventId { get; set; }

        [Column("CATEGORYNAME")]
        [Required, MaxLength(50)]
        public string CategoryName { get; set; } = "";

        [Column("DISTANCE")]
        public decimal Distance { get; set; }

        [Column("AGEGROUP")]
        [MaxLength(50)]
        public string? AgeGroup { get; set; }

        [Column("GENDERRESTRICTION")]
        [MaxLength(20)]
        public string? GenderRestriction { get; set; }

        [Column("PATICIPANTS")]          // typo in DB — kept as-is
        public int Participants { get; set; }

        [Column("MaxParticipants")]
        public int MaxParticipants { get; set; } = 50;

        [Column("Fee")]
        public decimal Fee { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Event? Event { get; set; }
        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }
}