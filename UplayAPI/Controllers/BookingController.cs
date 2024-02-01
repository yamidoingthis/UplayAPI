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
                || x.Activity.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            return Ok(list);
        }
        [HttpPost]
        public IActionResult AddBooking(Booking booking)
        {
            var now = DateTime.Now;
            var myBooking = new Booking()
            {
                Name = booking.Name.Trim(),
				Activity = booking.Activity.Trim(),
				Date = booking.Date,
                Time = booking.Time,
                Quantity = booking.Quantity,
                CreatedAt = now,
                UpdatedAt = now
            };
            _context.Bookings.Add(myBooking);
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
			myBooking.Activity = booking.Activity.Trim();
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
            _context.Bookings.Remove(myBooking);
            _context.SaveChanges();
            return Ok();
        }

    }
}
