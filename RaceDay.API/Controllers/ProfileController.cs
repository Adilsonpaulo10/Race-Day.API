
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Data;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ProfileController(RaceDayDbContext context)
        {
            _context = context;
        }

        // Both roles can view their own profile.
        [HttpGet("my")]
        public IActionResult GetMyProfile()
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = _context.Users.Find(userId);

            if (user == null)
                return NotFound(new { message = "Profile not found." });

            return Ok(new
            {
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }

        // Both roles can update their own profile.
        [HttpPut("my")]
        public IActionResult UpdateMyProfile(UpdateProfileRequest request)
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = _context.Users.Find(userId);

            if (user == null)
                return NotFound(new { message = "Profile not found." });

            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
            {
                return BadRequest(new
                {
                    message = "First name and last name are required."
                });
            }

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();

            _context.SaveChanges();

            return Ok(new
            {
                message = "Profile updated successfully.",
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }
    }

    public class UpdateProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
