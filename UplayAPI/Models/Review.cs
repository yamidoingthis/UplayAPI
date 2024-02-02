using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UplayAPI.Models
{
    public class Review
    {
        public int Id { get; set; }

        public int? RevStar { get; set; }

        [Required, MinLength(10), MaxLength(500)]
        public string RevDesc { get; set; } = string.Empty;

        public string RevStatus { get; set; } = "Unedited";

        public string RevFlag { get; set; } = "Not Flagged";

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        // Foreign key property
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        // Navigation property to represent the one-to-many relationship
        public User? User { get; set; }
        public Activity? Activity { get; set; }
    }
}
