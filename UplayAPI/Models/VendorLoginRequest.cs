using System.ComponentModel.DataAnnotations;

namespace UplayAPI.Models
{
	public class VendorLoginRequest
	{
		[Required, EmailAddress, MaxLength(50)]
		public string Email { get; set; } = string.Empty;
		[Required, MinLength(8), MaxLength(50)]
		public string Password { get; set; } = string.Empty;
	}
}
