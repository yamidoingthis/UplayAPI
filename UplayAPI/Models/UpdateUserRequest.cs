using System.ComponentModel.DataAnnotations;

namespace UplayAPI.Models
{
    public class UpdateUserRequest
    {
        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Phone number must 8 digits.")]
        public string Phone { get; set; }


        private string _nric;

        [RegularExpression(@"^\d{3}[A-Za-z]$", ErrorMessage = "NRIC must include last 3 digits and the letter.")]
        public string NRIC
        {
            get { return _nric; }
            set => _nric = value?.ToUpper();  // Convert to uppercase before storing


        }

        [Required(ErrorMessage = "Birth date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }


    }
}
