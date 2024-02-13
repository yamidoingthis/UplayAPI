using Microsoft.AspNetCore.Mvc;
using UplayAPI.Models;

namespace UplayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly MyDbContext _context;
        public BookingController(MyDbContext context)
        {
            _context = context;
        }
        

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Booking> result = _context.Bookings;
            if (search != null)
            {
                result = result.Where(x => x.Name.Contains(search)
                || x.Activity.Name.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            return Ok(list);
        }
        [HttpPost]
        public IActionResult AddBooking(Booking booking, int id)
        {
            var now = DateTime.Now; 
            var myBooking = new Booking()
            {
                Name = booking.Name.Trim(),
				Date = booking.Date,
                Time = booking.Time,
                Quantity = booking.Quantity,
                CreatedAt = now,
                UpdatedAt = now,
                ActivityId = id
                UpdatedAt = now,
                Status = "Unconfirmed",
                Price = booking.Price,
            };
            _context.Bookings.Add(myBooking);
			Models.Activity? activity = _context.Activities.FirstOrDefault(t => t.Id == id);
            Preferences preference = _context.Preferences.FirstOrDefault(t => t.Name == activity.Type);
            if (preference != null)
            {
                var newPreference = new Preferences()
                {
                    Name = activity.Type,
                    Bookings = 1
                };
                _context.Preferences.Add(newPreference);
            }
            else
            {
                preference.Bookings += 1;
            }
			_context.SaveChanges();
            return Ok(myBooking);
        }


        [HttpGet("{id}")]
        public IActionResult GetBooking(int id)
        {
            Booking? booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateBooking(int id, Booking booking)
        {
            var myBooking = _context.Bookings.Find(id);
            if (myBooking == null)
            {
                return NotFound();
            }
            myBooking.Name = booking.Name.Trim();
			myBooking.Date = booking.Date;
            myBooking.Time = booking.Time;
            myBooking.Quantity = booking.Quantity;
            myBooking.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteBooking(int id)
        {
            var myBooking = _context.Bookings.Find(id);
            if (myBooking == null)
            {
                return NotFound();
            }
			Models.Activity? activity = myBooking.Activity;
			Preferences preference = _context.Preferences.FirstOrDefault(t => t.Name == activity.Type);
            preference.Bookings -= 1;
			_context.Bookings.Remove(myBooking);
            _context.SaveChanges();
            return Ok();
        }

		[HttpPut("status/{id}")]
		public IActionResult UpdateStatus(int id)
		{
			var myBooking = _context.Bookings.Find(id);
			if (myBooking == null)
			{
				return NotFound();
			}
            myBooking.Status = "Confirmed";
			_context.SaveChanges();
			return Ok();
		}
	}
}
