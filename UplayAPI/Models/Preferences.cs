namespace UplayAPI.Models
{
	public class Preferences
	{
		public string Name { get; set; }
		public int Bookings { get; set; }
		// Foreign key property
		public int UserId { get; set; }
		// Navigation property to represent the one-to-many relationship
		public User? User { get; set; }
	}
}
