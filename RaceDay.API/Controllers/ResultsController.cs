
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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // Participants can view their own results.
        [HttpGet("my")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var results = await _context.Results
                .AsNoTracking()
                .Join(
                    _context.Enrolments,
                    result => result.EnrolmentID,
                    enrolment => enrolment.EnrolmentID,
                    (result, enrolment) => new { result, enrolment }
                )
                .Where(item => item.enrolment.UserID == userId)
                .Select(item => item.result)
                .ToListAsync();

            return Ok(results);
        }

        // Organisers can view all results.
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> GetAllResults()
        {
            var results = await _context.Results
                .AsNoTracking()
                .ToListAsync();

            return Ok(results);
        }

        // Organisers can record a result for an existing enrolment.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateResult(Result result)
        {
            bool enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentID == result.EnrolmentID);

            if (!enrolmentExists)
            {
                return BadRequest(new
                {
                    message = "The specified enrolment does not exist."
                });
            }

            bool resultExists = await _context.Results
                .AnyAsync(r => r.EnrolmentID == result.EnrolmentID);

            if (resultExists)
            {
                return Conflict(new
                {
                    message = "A result already exists for this enrolment."
                });
            }

            if (result.Position.HasValue && result.Position.Value <= 0)
            {
                return BadRequest(new
                {
                    message = "Position must be greater than zero."
                });
            }

            if (result.ResultStatus != "Finished" &&
                result.ResultStatus != "DNF" &&
                result.ResultStatus != "DNS")
            {
                return BadRequest(new
                {
                    message = "ResultStatus must be Finished, DNF, or DNS."
                });
            }

            result.ResultID = 0;

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Result recorded successfully.",
                result
            });
        }

        // Organisers can update an existing result.
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateResult(int id, Result updatedResult)
        {
            var result = await _context.Results
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
            {
                return NotFound(new { message = "Result not found." });
            }

            if (updatedResult.Position.HasValue &&
                updatedResult.Position.Value <= 0)
            {
                return BadRequest(new
                {
                    message = "Position must be greater than zero."
                });
            }

            if (updatedResult.ResultStatus != "Finished" &&
                updatedResult.ResultStatus != "DNF" &&
                updatedResult.ResultStatus != "DNS")
            {
                return BadRequest(new
                {
                    message = "ResultStatus must be Finished, DNF, or DNS."
                });
            }

            result.FinishTime = updatedResult.FinishTime;
            result.Position = updatedResult.Position;
            result.ResultStatus = updatedResult.ResultStatus;
            result.Notes = updatedResult.Notes;

            await _context.SaveChangesAsync();

            return Ok(result);
        }
    }
}
