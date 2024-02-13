using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace UplayAPI.Models
{
	public class Activity
	{
		public int Id { get; set; }
		[Required, MinLength(3), MaxLength(100)]
		public string Name { get; set; } = string.Empty;
		[Required, MinLength(3), MaxLength(100)]
		public string Type { get; set; } = string.Empty;
		[Required, MinLength(3), MaxLength(500)]
		public string Description { get; set; } = string.Empty;
		[MaxLength(20)]
		public string? ImageFile { get; set; }
		[Required, MinLength(3), MaxLength(100)]
		public string Location { get; set; } = string.Empty;
		[Required, MinLength(3), MaxLength(100)]
		public string Vendor { get; set; } = string.Empty;
		[Required]
		[Column(TypeName = "datetime")]
		public DateTime ActivityDate { get; set; }
		[Required]
		public float Price { get; set; }
		[Required]
		public Boolean IsActive { get; set; }
		[Column(TypeName = "datetime")]
		public DateTime CreatedAt { get; set; }
		[Column(TypeName = "datetime")]
		public DateTime UpdatedAt { get; set; }
		// Foreign key property
		public int UserId { get; set; }
		// Navigation property to represent the one-to-many relationship
		public User? User { get; set; }
		[JsonIgnore]
		public List<Booking>? Bookings { get; set; }
	}
}
