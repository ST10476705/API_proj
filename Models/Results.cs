using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_proj.Models
{
    [Table("RESULTS")]
    public class Result
    {
        [Key]
        [Column("RESULTSID")]
        public int ResultsId { get; set; }

        [Column("ENROLMENTID")]
        public int EnrolmentId { get; set; }

        [Column("CATEGORIESID")]         // NOT NULL in DB — must be set
        public int CategoryId { get; set; }

        [Column("FINISHTIME")]
        public TimeSpan? FinishTime { get; set; }

        [Column("OVERALLRANK")]
        public int? OverallRank { get; set; }

        [Column("CATEGORYRANK")]
        public int? CategoryRank { get; set; }

        [Column("NOTES")]
        [MaxLength(255)]
        public string? Notes { get; set; }

        [Column("Status")]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        [Column("VerifiedBy")]
        public int? VerifiedBy { get; set; }

        [Column("VerifiedAt")]
        public DateTime? VerifiedAt { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public Enrolment? Enrolment { get; set; }
        public Category? Category { get; set; }
    }
}