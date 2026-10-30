using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_proj.Models
{
    [Table("EVENTS")]
    public class Event
    {
        [Key]
        [Column("EVENTSDID")]
        public int EventId { get; set; }

        [Column("EVENTNAME")]
        [Required, MaxLength(100)]
        public string EventName { get; set; } = "";

        [Column("EVENTDATE")]
        public DateTime EventDate { get; set; }

        [Column("VENUE")]
        [Required, MaxLength(100)]
        public string Venue { get; set; } = "";

        [Column("LOCATION")]
        [MaxLength(255)]
        public string? Location { get; set; }

        [Column("DESCRIPTION")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Column("USERID")]
        public int UserId { get; set; }

        [Column("MaxParticipants")]
        public int MaxParticipants { get; set; } = 100;

        [Column("RegistrationFee")]
        public decimal RegistrationFee { get; set; }

        [Column("Status")]
        [MaxLength(20)]
        public string Status { get; set; } = "Draft";

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}