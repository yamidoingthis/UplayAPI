using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UplayAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(8)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(4)]
        public string NRIC { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }


        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [JsonIgnore]
        public List<Review>? Reviews { get; set; }
        public List<Booking>? Bookings { get; set; }
    }
}
