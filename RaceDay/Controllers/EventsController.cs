using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventApiService _eventApiService;

        public EventsController(EventApiService eventApiService)
        {
            _eventApiService = eventApiService;
        }

        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var events = await _eventApiService.GetEventsAsync(token);

                return View(events);
            }
            catch (HttpRequestException)
            {
                ViewBag.Error = "Unable to retrieve events from the API.";
                return View(new List<EventDto>());
            }
        }
    }
}