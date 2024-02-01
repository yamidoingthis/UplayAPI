using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UplayAPI.Models
{
    public class Complaint
    {
        public int Id { get; set; }

        [Required, MinLength(5), MaxLength(100)]
        public string ComTitle { get; set; } = string.Empty;

        [Required, MinLength(10), MaxLength(2000)]
        public string ComDesc { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ComSugg { get; set; } = string.Empty;

        public string ComStatus { get; set; } = "Unresolved";

        public string ComResp { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime RespondedAt { get; set; }

        // Foreign key property
        public int UserId { get; set; }

        // Navigation property to represent the one-to-many relationship
        public User? User { get; set; }
    }
}
