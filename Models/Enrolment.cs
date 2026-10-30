using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_proj.Models
{
    [Table("ENROLMENTS")]
    public class Enrolment
    {
        [Key]
        [Column("ENROLMENTID")]
        public int EnrolmentId { get; set; }

        [Column("USERID")]
        public int UserId { get; set; }

        [Column("CATEGORIESID")]
        public int CategoryId { get; set; }

        [Column("RegistrationDate")]
        public DateTime RegistrationDate { get; set; }

        [Column("Status")]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [Column("PaymentStatus")]
        [MaxLength(20)]
        public string PaymentStatus { get; set; } = "Unpaid";

        [Column("BibNumber")]
        [MaxLength(20)]
        public string? BibNumber { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Navigation
        // NOTE: no EventId — the Event is reached via Category.Event
        public User? User { get; set; }
        public Category? Category { get; set; }
    }
}