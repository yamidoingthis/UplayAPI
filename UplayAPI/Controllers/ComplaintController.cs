using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UplayAPI.Models;

namespace UplayAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ComplaintController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ComplaintController(MyDbContext context)
        {
            _context = context;
        }

        [HttpPost, Authorize]
        public IActionResult AddComplaint(Complaint complaint)
        {
            int userId = GetUserId();
            var now = DateTime.Now;
            var myComplaint = new Complaint()
            {
                ComTitle = complaint.ComTitle.Trim(),
                ComDesc = complaint.ComDesc.Trim(),
                ComSugg = complaint.ComSugg.Trim(),
                ComStatus = "Unaddressed",
                ComResp = "",
                CreatedAt = now,
                UpdatedAt = now,
                RespondedAt = now,
                UserId = userId
            };

            _context.Complaints.Add(myComplaint);
            _context.SaveChanges();
            return Ok(myComplaint);
        }

        [HttpGet]
        public IActionResult GetAll(string? search)
        {
            IQueryable<Complaint> result = _context.Complaints.Include(c => c.User);
            if (search != null)
            {
                result = result.Where(x => x.ComDesc.Contains(search));
            }
            var list = result.OrderByDescending(x => x.CreatedAt).ToList();
            var data = list.Select(c => new
            {
                c.Id,
                c.ComTitle,
                c.ComDesc,
                c.ComSugg,
                c.ComStatus,
                c.ComResp,
                c.CreatedAt,
                c.UpdatedAt,
                c.RespondedAt,
                c.UserId,
                User = new
                {
                    c.User?.Name
                }
            });
            return Ok(data);
        }

        [HttpGet("{id}")]
        public IActionResult GetComplaint(int id)
        {
            Complaint? complaint = _context.Complaints.Include(c => c.User)
                .FirstOrDefault(c => c.Id == id);
            if (complaint == null)
            {
                return NotFound();
            }
            var data = new
            {
                complaint.Id,
                complaint.ComTitle,
                complaint.ComDesc,
                complaint.ComSugg,
                complaint.ComStatus,
                complaint.ComResp,
                complaint.CreatedAt,
                complaint.UpdatedAt,
                complaint.RespondedAt,
                complaint.UserId,
                User = new
                {
                    complaint.User?.Name
                }
            };
            return Ok(data);
        }

        [HttpPut("{id}"), Authorize]
        public IActionResult UpdateComplaint(int id, Complaint complaint)
        {
            var myComplaint = _context.Complaints.Find(id);
            if (myComplaint == null)
            {
                return NotFound();
            }

            int userId = GetUserId();
            if (myComplaint.UserId != userId)
            {
                return Forbid();
            }

            myComplaint.ComTitle = complaint.ComTitle.Trim();
            myComplaint.ComDesc = complaint.ComDesc.Trim();
            myComplaint.ComSugg = complaint.ComSugg.Trim();
            myComplaint.ComStatus = "Unaddressed";
            myComplaint.ComResp = "";
            myComplaint.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok();
        }

        [HttpPut("respond/{id}")]
        public IActionResult RespondComplaint(int id, Complaint complaint)
        {
            var myComplaint = _context.Complaints.Find(id);
            if (myComplaint == null)
            {
                return NotFound();
            }

            myComplaint.ComTitle = complaint.ComTitle.Trim();
            myComplaint.ComDesc = complaint.ComDesc.Trim();
            myComplaint.ComSugg = complaint.ComSugg.Trim();
            myComplaint.ComStatus = "Addressed";
            myComplaint.ComResp = complaint.ComResp.Trim();
            myComplaint.RespondedAt = DateTime.Now;

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            var myComplaint = _context.Complaints.Find(id);
            if (myComplaint == null)
            {
                return NotFound();
            }

            int userId = GetUserId();
            if (myComplaint.UserId != userId)
            {
                return Forbid();
            }

            _context.Complaints.Remove(myComplaint);
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
