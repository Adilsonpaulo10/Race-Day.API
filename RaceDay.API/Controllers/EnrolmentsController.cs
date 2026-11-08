
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using System.Security.Claims;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // Participants can view their own enrolments.
        [HttpGet("my")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyEnrolments()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.UserID == userId)
                .ToListAsync();

            return Ok(enrolments);
        }

        // Organisers can view all enrolments.
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetAllEnrolments()
        {
            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .ToListAsync();

            return Ok(enrolments);
        }

        // Participants can enrol themselves in a category.
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> CreateEnrolment(Enrolment enrolment)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            bool categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryID == enrolment.CategoryID);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    message = "The specified category does not exist."
                });
            }

            bool alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e => e.UserID == userId &&
                               e.CategoryID == enrolment.CategoryID);

            if (alreadyEnrolled)
            {
                return Conflict(new
                {
                    message = "You are already enrolled in this category."
                });
            }

            enrolment.EnrolmentID = 0;
            enrolment.UserID = userId;
            enrolment.EnrolmentDate = DateTime.Now;
            enrolment.Status = "Registered";

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Enrolment created successfully.",
                enrolment
            });
        }

        // Participants can withdraw from their own enrolments.
        [HttpPut("{id}/withdraw")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> WithdrawEnrolment(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var enrolment = await _context.Enrolments
                .FirstOrDefaultAsync(e => e.EnrolmentID == id &&
                                          e.UserID == userId);

            if (enrolment == null)
            {
                return NotFound(new
                {
                    message = "Enrolment not found for this participant."
                });
            }

            enrolment.Status = "Withdrawn";
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Enrolment withdrawn successfully.",
                enrolment
            });
        }
    }
}
