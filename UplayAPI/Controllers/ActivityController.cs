using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using UplayAPI.Models;

namespace UplayAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class ActivityController : ControllerBase
	{
		private readonly MyDbContext _context;
		public ActivityController(MyDbContext context)
		{
			_context = context;
		}
		private int GetUserId()
		{
			return Convert.ToInt32(User.Claims
				.Where(c => c.Type == ClaimTypes.NameIdentifier)
				.Select(c => c.Value).SingleOrDefault());
		}
		[HttpGet]
		public IActionResult GetAll(string? search)
		{
			IQueryable<Activity> result = _context.Activities.Include(t => t.User).Where(x => x.IsActive == true);
			if (search != null)
			{
				result = result.Where(x => x.Name.Contains(search)
				|| x.Description.Contains(search));
			}
			var list = result.OrderByDescending(x => x.CreatedAt).ToList();
			var data = list.Select(t => new
			{
				t.Id,
				t.Name,
				t.Type,
				t.Description,
				t.ImageFile,
				t.Location,
				t.ActivityDate,
				t.Price,
				t.CreatedAt,
				t.UpdatedAt,
				t.UserId,
                User = new
				{
					t.User?.Name
				}
			});
			return Ok(data);
		}
		[HttpGet("sortedpriceascending")]
		public IActionResult GetAllPriceAscendingSorted()
		{
			IQueryable<Activity> result = _context.Activities.Include(t => t.User).Where(x => x.IsActive == true);
			var list = result.OrderByDescending(x => x.Price).ToList();
			list.Reverse();
			var data = list.Select(t => new
			{
				t.Id,
				t.Name,
				t.Type,
				t.Description,
				t.ImageFile,
				t.Location,
				t.ActivityDate,
				t.Price,
				t.CreatedAt,
				t.UpdatedAt,
				t.UserId,
                User = new
				{
					t.User?.Name
				}
			});
			return Ok(data);
		}
		[HttpGet("sortedpricedescending")]
		public IActionResult GetAllPriceDescendingSorted()
		{
			IQueryable<Activity> result = _context.Activities.Include(t => t.User).Where(x => x.IsActive == true);
			var list = result.OrderByDescending(x => x.Price).ToList();
			var data = list.Select(t => new
			{
				t.Id,
				t.Name,
				t.Type,
				t.Description,
				t.ImageFile,
				t.Location,
				t.ActivityDate,
				t.Price,
				t.CreatedAt,
				t.UpdatedAt,
				t.UserId,
                User = new
				{
					t.User?.Name
				}
			});
			return Ok(data);
		}
		[HttpGet("{id}")]
		public IActionResult GetActivity(int id)
		{
			Activity? activity = _context.Activities.Include(t => t.User)
				.FirstOrDefault(t => t.Id == id);
			if (activity == null || activity.IsActive == false)
			{
				return NotFound();
			}
			var data = new
			{
				activity.Id,
				activity.Name,
				activity.Type,
				activity.Description,
				activity.ImageFile,
				activity.Location,
				activity.ActivityDate,
				activity.Price,
				activity.CreatedAt,
				activity.UpdatedAt,
				activity.UserId,
                User = new
				{
					activity.User?.Name
				}
			};
			return Ok(data);
		}
		[HttpPost, Authorize]
		public IActionResult AddActivity(Activity activity)
		{
			int userId = GetUserId();
			var now = DateTime.Now;
			var myActivity = new Activity()
			{
				Name = activity.Name.Trim(),
				Type = activity.Type.Trim(),
				Description = activity.Description.Trim(),
				ImageFile = activity.ImageFile,
				Location = activity.Location.Trim(),
				ActivityDate = activity.ActivityDate,
				Price = activity.Price,
				IsActive = true,
				CreatedAt = now,
				UpdatedAt = now,
                UserId = userId
            };
			_context.Activities.Add(myActivity);
			_context.SaveChanges();
			return Ok(myActivity);
		}
		[HttpPut("restore/{id}"), Authorize]
		public IActionResult RestoreActivity(int id, Activity activity)
		{
			var myActivity = _context.Activities.Find(id);
			if (myActivity == null || myActivity.IsActive == true)
			{
				return NotFound();
			}
			int userId = GetUserId();
			if (myActivity.UserId != userId)
			{
				return Forbid();
			}
			myActivity.IsActive = true;
			_context.SaveChanges();
			return Ok();
		}
		[HttpPut("{id}"), Authorize]
		public IActionResult UpdateActivity(int id, Activity activity)
		{
			var myActivity = _context.Activities.Find(id);
			if (myActivity == null || myActivity.IsActive == false)
			{
				return NotFound();
			}
			int userId = GetUserId();
			if (myActivity.UserId != userId)
			{
				return Forbid();
			}
			myActivity.Name = activity.Name.Trim();
			myActivity.Description = activity.Description.Trim();
			myActivity.ImageFile = activity.ImageFile;
			myActivity.Location = activity.Location.Trim();
			myActivity.UpdatedAt = DateTime.Now;
			_context.SaveChanges();
			return Ok();
		}
		[HttpDelete("{id}"), Authorize]
		public IActionResult DeleteActivity(int id)
		{
			var myActivity = _context.Activities.Find(id);
			if (myActivity == null || myActivity.IsActive == false)
			{
				return NotFound();
			}
			int userId = GetUserId();
			if (myActivity.UserId != userId)
			{
				return Forbid();
			}
			myActivity.IsActive = false;
			_context.SaveChanges();
			return Ok();
		}
	}
}
