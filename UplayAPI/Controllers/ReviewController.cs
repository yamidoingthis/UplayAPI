using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UplayAPI.Models;

namespace UplayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ReviewController(MyDbContext context)
        {
            _context = context;
        }

        [HttpPost("{id}"), Authorize]
        public IActionResult AddReview(Review review, int id)
        {
            int userId = GetUserId();
            int activityId = id;
            var now = DateTime.Now;
            var myReview = new Review()
            {
                RevStar = review.RevStar,
                RevDesc = review.RevDesc.Trim(),
                RevStatus = "Unedited",
                RevFlag = "Not Flagged",
                CreatedAt = now,
                UpdatedAt = now,
                UserId = userId,
                ActivityId = activityId
            };

            _context.Reviews.Add(myReview);
            _context.SaveChanges();
            return Ok(myReview);
        }

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Review> result = _context.Reviews.Include(r => r.User);
            if (search != null)
            {
                result = result.Where(x => x.RevDesc.Contains(search));
            }
            var myList = result.OrderByDescending(y => y.CreatedAt).ToList();
            var data = myList.Select(r => new
            {
                r.Id,
                r.RevStar,
                r.RevDesc,
                r.RevStatus,
                r.RevFlag,
                r.CreatedAt,
                r.UpdatedAt,
                r.UserId,
                User = new
                {
                    r.User?.Name
                },
                r.ActivityId,
                Activity = new
                {
                    r.Activity?.Name
                }
            });
            return Ok(data);
        }

        [HttpGet("activity/{id}")]
        public IActionResult GetById(int id)
        {
            IQueryable<Review> result = _context.Reviews.Include(r => r.User).Where(r => r.ActivityId == id);

            var myList = result.OrderByDescending(y => y.CreatedAt).ToList();
            var data = myList.Select(r => new
            {
                r.Id,
                r.RevStar,
                r.RevDesc,
                r.RevStatus,
                r.RevFlag,
                r.CreatedAt,
                r.UpdatedAt,
                r.UserId,
                User = new
                {
                    r.User?.Name
                },
                r.ActivityId,
                Activity = new
                {
                    r.Activity?.Name
                }
            });
            return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult GetReview(int id)
        {
            Review? review = _context.Reviews.Include(r => r.User)
                .FirstOrDefault(r => r.Id == id);
            if (review == null)
            {
                return NotFound();
            }
            var data = new
            {
                review.Id,
                review.RevStar,
                review.RevDesc,
                review.RevStatus,
                review.RevFlag,
                review.CreatedAt,
                review.UpdatedAt,
                review.UserId,
                User = new
                {
                    review.User?.Name
                },
                review.ActivityId,
                Activity = new 
                { 
                    review.Activity?.Name
                }
            };
            return Ok(data);
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateReview(int id, Review review)
        {
            var myReview = _context.Reviews.Find(id);
            if (myReview == null)
            {
                return NotFound();
            }

            myReview.RevStar = review.RevStar;
            myReview.RevDesc = review.RevDesc.Trim();
            myReview.RevStatus = review.RevStatus;
            myReview.RevFlag = review.RevFlag;
            myReview.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            var myReview = _context.Reviews.Find(id);
            if (myReview == null)
            {
                return NotFound();
            }

            int userId = GetUserId();
            if (myReview.UserId != userId)
            {
                return Forbid();
            }

            myReview.RevStatus = "Deleted";
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("flag/{id}"), Authorize]
        public IActionResult Flag(int id)
        {
            var myReview = _context.Reviews.Find(id);
            if (myReview == null)
            {
                return NotFound();
            }

            myReview.RevFlag = "Flagged";
            _context.SaveChanges();
            return Ok();
        }

        [HttpPut("approve/{id}"), Authorize]
        public IActionResult Approve(int id)
        {
            var myReview = _context.Reviews.Find(id);
            if (myReview == null)
            {
                return NotFound();
            }

            myReview.RevFlag = "Not Flagged";
            myReview.RevStatus = "Unedited";
            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("hide/{id}"), Authorize]
        public IActionResult Hide(int id)
        {
            var myReview = _context.Reviews.Find(id);
            if (myReview == null)
            {
                return NotFound();
            }

            myReview.RevStatus = "Hidden";
            _context.SaveChanges();
            return Ok();
        }

        private int GetUserId()
        {
            return Convert.ToInt32(User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault());
        }
    }
}
