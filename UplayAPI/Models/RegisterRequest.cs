using System.ComponentModel.DataAnnotations;

namespace UplayAPI.Models
{
    public class RegisterRequest
    {
        [Required, MinLength(3), MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z '-,.]+$", ErrorMessage = "Only allow letters, spaces and characters: ' - , .")]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(50)]
        [RegularExpression(@"^(?=.*[a-zA-Z]).{8,}$", ErrorMessage = "At least 1 letter")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Phone number must 8 digits.")]
        public string Phone { get; set; }


        private string _nric;

        [RegularExpression(@"^\d{3}[A-Za-z]$", ErrorMessage = "NRIC must include last 3 digits and the letter.")]
        public string NRIC
        {
            get { return _nric; }
            set => _nric = value?.ToUpper();
        }

        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }



        [Required, MinLength(8), MaxLength(50)]
        public string Password { get; set; } = string.Empty;

    }
}
