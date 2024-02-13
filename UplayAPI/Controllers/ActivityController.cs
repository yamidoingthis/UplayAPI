using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
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
			IQueryable<Activity> result = _context.Activities.Include(t => t.Vendor).Where(x => x.IsActive == true);
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
				t.Vendor,
				t.ActivityDate,
				t.Price,
				t.CreatedAt,
				t.UpdatedAt
			});
			return Ok(data);
		}
		[HttpGet("recommended")]
		public IActionResult GetRecommended()
		{
			int userId = GetUserId();
			IQueryable<Preferences> user = _context.Preferences.Where(x => x.UserId == userId);
			var preferences = user.OrderByDescending(x => x.Bookings).ToList();
			IQueryable<Models.Activity> result = _context.Activities.Where(x => x.IsActive == true);
			var list = result.OrderByDescending(x => x.Price).ToList();
			var recommended = new List<Models.Activity>();
			foreach (var item in preferences)
			{
				foreach(var activity in list)
				{
					if (activity.Type == item.Name)
					{
						recommended.Add(activity);
					}
				}
			}
			var data = recommended.Select(t => new
			{
				t.Id,
				t.Name,
				t.Type,
				t.Description,
				t.ImageFile,
				t.Location,
				t.Vendor,
				t.ActivityDate,
				t.Price,
				t.CreatedAt,
				t.UpdatedAt
			});
			return Ok(data);
		}
		[HttpGet("sortedpriceascending")]
		public IActionResult GetAllPriceAscendingSorted()
		{
			IQueryable<Models.Activity> result = _context.Activities.Where(x => x.IsActive == true);
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
				t.Vendor,
				t.ActivityDate,
				t.Price,
				t.CreatedAt,
				t.UpdatedAt
			});
			return Ok(data);
		}
		[HttpGet("sortedpricedescending")]
		public IActionResult GetAllPriceDescendingSorted()
		{
			IQueryable<Models.Activity> result = _context.Activities.Where(x => x.IsActive == true);
			var list = result.OrderByDescending(x => x.Price).ToList();
			var data = list.Select(t => new
			{
				t.Id,
				t.Name,
				t.Type,
				t.Description,
				t.ImageFile,
				t.Location,
				t.Vendor,
				t.ActivityDate,
				t.Price,
				t.CreatedAt,
				t.UpdatedAt
			});
			return Ok(data);
		}
		[HttpGet("{id}")]
		public IActionResult GetActivity(int id)
		{
            Models.Activity? activity = _context.Activities.FirstOrDefault(t => t.Id == id);
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
				activity.Vendor,
				activity.ActivityDate,
				activity.Price,
				activity.CreatedAt,
				activity.UpdatedAt
			};
			return Ok(data);
		}
		[HttpPost, Authorize]
		public IActionResult AddActivity(Models.Activity activity)
		{
			int vendorId = GetUserId();
			var now = DateTime.Now;
			var myActivity = new Models.Activity()
			{
				Name = activity.Name.Trim(),
				Type = activity.Type.Trim(),
				Description = activity.Description.Trim(),
				ImageFile = activity.ImageFile,
				Location = activity.Location.Trim(),
				Vendor = activity.Vendor,
				ActivityDate = activity.ActivityDate,
				Price = activity.Price,
				IsActive = true,
				CreatedAt = now,
				UpdatedAt = now,
				UserId = vendorId
			};
			_context.Activities.Add(myActivity);
			_context.SaveChanges();
			return Ok(myActivity);
		}
		[HttpPut("restore/{id}"), Authorize]
		public IActionResult RestoreActivity(int id, Models.Activity activity)
		{
			var myActivity = _context.Activities.Find(id);
			if (myActivity == null || myActivity.IsActive == true)
			{
				return NotFound();
			}
			int vendorId = GetUserId();
			if (myActivity.UserId != vendorId)
			{
				return Forbid();
			}
			myActivity.IsActive = true;
			_context.SaveChanges();
			return Ok();
		}
		[HttpPut("{id}"), Authorize]
		public IActionResult UpdateActivity(int id, Models.Activity activity)
		{
			var myActivity = _context.Activities.Find(id);
			if (myActivity == null || myActivity.IsActive == false)
			{
				return NotFound();
			}
			int vendorId = GetUserId();
			if (myActivity.UserId != vendorId)
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
			int vendorId = GetUserId();
			if (myActivity.UserId != vendorId)
			{
				return Forbid();
			}
			myActivity.IsActive = false;
			_context.SaveChanges();
			return Ok();
		}
	}
}
