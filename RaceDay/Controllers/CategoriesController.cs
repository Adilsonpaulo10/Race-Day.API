using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly CategoryApiService _categoryApiService;

        public CategoriesController(CategoryApiService categoryApiService)
        {
            _categoryApiService = categoryApiService;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            try
            {
                var categories =
                    await _categoryApiService.GetCategoriesAsync(token);

                return View(categories);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to retrieve categories from the API.";
                return View(new List<CategoryDto>());
            }
        }
    }
}