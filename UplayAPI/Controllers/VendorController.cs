using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UplayAPI.Models;

namespace UplayAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class VendorController : ControllerBase
	{
		private readonly MyDbContext _context;
		private readonly IConfiguration _configuration;
		public VendorController(MyDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}
		private string CreateToken(Vendor vendor)
		{
			string secret = _configuration.GetValue<string>(
"Authentication:Secret");
			int tokenExpiresDays = _configuration.GetValue<int>(
"Authentication:TokenExpiresDays");
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secret);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.NameIdentifier, vendor.Id.ToString()),
					new Claim(ClaimTypes.Name, vendor.Name),
					new Claim(ClaimTypes.Email, vendor.Email)
				}),
				Expires = DateTime.UtcNow.AddDays(tokenExpiresDays),
				SigningCredentials = new SigningCredentials(
new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var securityToken = tokenHandler.CreateToken(tokenDescriptor);
			string token = tokenHandler.WriteToken(securityToken);
			return token;
		}
		[HttpPost("register")]
		public IActionResult VendorRegister(VendorRegisterRequest request)
		{
			// Trim string values
			request.Name = request.Name.Trim();
			request.Email = request.Email.Trim().ToLower();
			request.Password = request.Password.Trim();
			// Check email
			var foundVendor = _context.Vendors.Where(
				x => x.Email == request.Email).FirstOrDefault();
			if (foundVendor != null)
			{
				string message = "Email already exists.";
				return BadRequest(new { message });
			}
			// Create vendor object
			var now = DateTime.Now;
			string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
			var vendor = new Vendor()
			{
				Name = request.Name,
				Email = request.Email,
				Password = passwordHash,
				CreatedAt = now,
				UpdatedAt = now
			};
			// Add vendor
			_context.Vendors.Add(vendor);
			_context.SaveChanges();
			return Ok();
		}
		[HttpPost("login")]
		public IActionResult Login(VendorLoginRequest request)
		{
			// Trim string values
			request.Email = request.Email.Trim().ToLower();
			request.Password = request.Password.Trim();
			// Check email and password
			string message = "Email or password is not correct.";
			var foundVendor = _context.Vendors.Where(
				x => x.Email == request.Email).FirstOrDefault();
			if (foundVendor == null)
			{
				return BadRequest(new { message });
			}
			bool verified = BCrypt.Net.BCrypt.Verify(
			request.Password, foundVendor.Password);
			if (!verified)
			{
				return BadRequest(new { message });
			}
			// Return vendor info
			var vendor = new
			{
				foundVendor.Id,
				foundVendor.Email,
				foundVendor.Name
			};
			string accessToken = CreateToken(foundVendor);
			return Ok(new { vendor, accessToken });
		}
		[HttpGet("auth"), Authorize]
		public IActionResult Auth()
		{
			var id = Convert.ToInt32(User.Claims.Where(
				c => c.Type == ClaimTypes.NameIdentifier)
				.Select(c => c.Value).SingleOrDefault());
			var name = User.Claims.Where(c => c.Type == ClaimTypes.Name)
				.Select(c => c.Value).SingleOrDefault();
			var email = User.Claims.Where(c => c.Type == ClaimTypes.Email)
				.Select(c => c.Value).SingleOrDefault();
			if (id != 0 && name != null && email != null)
			{
				var vendor = new
				{
					id,
					email,
					name
				};
				return Ok(new { vendor });
			}
			else
			{
				return Unauthorized();
			}
		}
	}
}
