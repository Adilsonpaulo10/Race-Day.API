using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountApiService _accountApiService;

        public AccountController(AccountApiService accountApiService)
        {
            _accountApiService = accountApiService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _accountApiService.LoginAsync(model);

            if (result == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("UserID", result.UserID.ToString());
            HttpContext.Session.SetString("FirstName", result.FirstName);
            HttpContext.Session.SetString("LastName", result.LastName);
            HttpContext.Session.SetString("Email", result.Email);
            HttpContext.Session.SetString("Role", result.Role);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}