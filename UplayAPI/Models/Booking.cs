using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UplayAPI.Models
{
    public class Booking
    {
        public int Id { get; set; }
        [Required, MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;
		public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
       
        public int Quantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        public string Status {  get; set; } = string.Empty;
        // Foreign key property
        public int ActivityId { get; set; }
        public int UserId { get; set; }

        // Navigation property to represent the one-to-many relationship
        public User? User { get; set; }

    }
}
