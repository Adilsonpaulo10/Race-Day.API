
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // Both Participants and Organisers can view categories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .ToListAsync();

            return Ok(categories);
        }

        // View categories for a specific event
        [HttpGet("event/{eventId}")]
        public async Task<IActionResult> GetCategoriesByEvent(int eventId)
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Where(c => c.EventID == eventId)
                .ToListAsync();

            return Ok(categories);
        }

        // View one category
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            return Ok(category);
        }

        // Only Organisers can create categories
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == category.EventID);

            if (!eventExists)
            {
                return BadRequest(new
                {
                    message = "The specified event does not exist."
                });
            }

            category.CategoryID = 0;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryID },
                category);
        }

        // Only Organisers can update categories
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int id, Category updatedCategory)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == updatedCategory.EventID);

            if (!eventExists)
            {
                return BadRequest(new
                {
                    message = "The specified event does not exist."
                });
            }

            category.EventID = updatedCategory.EventID;
            category.CategoryName = updatedCategory.CategoryName;
            category.Distance = updatedCategory.Distance;
            category.EntryFee = updatedCategory.EntryFee;

            await _context.SaveChangesAsync();

            return Ok(category);
        }

        // Only Organisers can delete categories
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound(new
                {
                    message = "Category not found."
                });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
