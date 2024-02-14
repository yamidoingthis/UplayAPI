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
    public class UserController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IConfiguration _configuration;
        public UserController(MyDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }




        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            request.Name = request.Name.Trim();
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();
            request.Phone = request.Phone.Trim();
            request.NRIC = request.NRIC.Trim();

            var foundUser = _context.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (foundUser != null)
            {
                string message = "Email already exists.";
                return BadRequest(new { message });
            }

            DateTime? birthDate = null;
            if (DateTime.TryParseExact(request.BirthDate?.ToString("dd-MM-yyyy"), "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out var parsedDate))
            {
                birthDate = parsedDate;
            }
            else
            {
                // Handle invalid date format
                string message = "Invalid birth date format.";
                return BadRequest(new { message });
            }

            // Create user object
            var now = DateTime.Now;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User()
            {
                Name = request.Name,
                Email = request.Email,
                Password = passwordHash,
                Phone = request.Phone,
                NRIC = request.NRIC,
                BirthDate = birthDate,
                CreatedAt = now,
                UpdatedAt = now
            };
            // Add user
            _context.Users.Add(user);
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            // Trim string values
            request.Email = request.Email.Trim().ToLower();
            request.Password = request.Password.Trim();
            // Check email and password
            string message = "Email or password is not correct.";
            var foundUser = _context.Users.Where(x => x.Email == request.Email).FirstOrDefault();
            if (foundUser == null)
            {
                return BadRequest(new { message });
            }
            bool verified = BCrypt.Net.BCrypt.Verify(
            request.Password, foundUser.Password);
            if (!verified)
            {
                return BadRequest(new { message });
            }
            // Return user info
            var user = new
            {
                foundUser.Id,
                foundUser.Email,
                foundUser.Name
            };
            string accessToken = CreateToken(foundUser);
            return Ok(new { user, accessToken });
        }

        private string CreateToken(User user)
        {
            string secret = _configuration.GetValue<string>("Authentication:Secret");
            int tokenExpiresDays = _configuration.GetValue<int>("Authentication:TokenExpiresDays");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(tokenExpiresDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
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
                var user = new
                {
                    id,
                    email,
                    name,

                };
                return Ok(new { user });
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            User? user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet("all")]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpPost("confirm-password")]
        [Authorize] // Make sure the user is authenticated
        public IActionResult ConfirmPassword(ConfirmPasswordRequest request)
        {
            var userId = Convert.ToInt32(User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .SingleOrDefault());

            var foundUser = _context.Users.Find(userId);

            // Verify the provided password against the stored hashed password
            bool verified = BCrypt.Net.BCrypt.Verify(request.Password, foundUser.Password);

            if (verified)
            {
                // Password is correct, return success response
                return Ok(new { message = "Password confirmed successfully." });
            }
            else
            {
                // Password is incorrect, return error response
                return BadRequest(new { message = "Incorrect password." });
            }
        }

        [HttpPost("change-password")]
        [Authorize] // Make sure the user is authenticated
        public IActionResult ChangePassword(ChangePasswordRequest request)
        {
            var userId = Convert.ToInt32(User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .SingleOrDefault());

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Verify if the new password is different from the current password
            bool isDifferent = !BCrypt.Net.BCrypt.Verify(request.NewPassword, user.Password);
            if (!isDifferent)
            {
                return BadRequest("New password must be different from the current password");
            }

            // Hash the new password
            string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            // Update user's password
            user.Password = newPasswordHash;
            _context.SaveChanges();

            return Ok("Password updated successfully");
        }



        [HttpPut("update/{id}"), Authorize]
        public IActionResult UpdateProfile(int id, UpdateUserRequest request)
        {
            var userId = Convert.ToInt32(User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .SingleOrDefault());

            if (userId != id)
            {
                return Forbid();
            }

            var existingUser = _context.Users.Find(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            
            if (!string.IsNullOrEmpty(request.Phone))
            {
                existingUser.Phone = request.Phone;
            }

            if (!string.IsNullOrEmpty(request.NRIC))
            {
                existingUser.NRIC = request.NRIC;
            }

            if (request.BirthDate.HasValue)
            {
                existingUser.BirthDate = request.BirthDate;
            }

            _context.SaveChanges();

            return Ok();
        }


        [HttpDelete("delete/{id}"), Authorize]
        public IActionResult DeleteUser(int id)
        {
            var userId = Convert.ToInt32(User.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value)
                .SingleOrDefault());

            if (userId != id)
            {
                return Forbid();
            }

            var existingUser = _context.Users.Find(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(existingUser);
            _context.SaveChanges();

            return Ok();
        }

    }
}
