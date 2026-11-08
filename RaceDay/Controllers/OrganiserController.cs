using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;
using System.Net.Http;

namespace RaceDay.Web.Controllers
{

    public class OrganiserController : Controller
    {
        private readonly EventApiService _eventApiService;

        public OrganiserController(EventApiService eventApiService)
        {
            _eventApiService = eventApiService;
        }
        public async Task<IActionResult> Dashboard()
        {
            var token = HttpContext.Session.GetString("Token");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (role != "Organiser")
            {
                return RedirectToAction("Index", "Home");
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
        public IActionResult CreateEvent()
{
    if (HttpContext.Session.GetString("Role") != "Organiser")
    {
        return RedirectToAction("Index", "Home");
    }

    return View();

}
        public async Task<IActionResult> ManageEvents()
        {
            var token = HttpContext.Session.GetString("Token");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Organiser")
                return RedirectToAction("Index", "Home");

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
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            try
            {
                var events = await _eventApiService.GetEventsAsync(token);
                var eventItem = events.FirstOrDefault(e => e.EventID == id);

                if (eventItem == null)
                    return NotFound();

                var model = new CreateEventViewModel
                {
                    EventName = eventItem.EventName,
                    Description = eventItem.Description,
                    EventDate = eventItem.EventDate,
                    Location = eventItem.Location,
                    Status = eventItem.Status
                };

                return View(model);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction("ManageEvents");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(int id, CreateEventViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            if (!ModelState.IsValid)
                return View(model);

            var success = await _eventApiService.UpdateEventAsync(id, model, token);

            if (!success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update event. Please try again.");

                return View(model);
            }

            TempData["Success"] = "Event updated successfully!";
            return RedirectToAction("ManageEvents");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Index", "Home");

            var success = await _eventApiService.DeleteEventAsync(id, token);

            if (!success)
            {
                TempData["Error"] = "Unable to delete event.";
                return RedirectToAction("ManageEvents");
            }

            TempData["Success"] = "Event deleted successfully!";
            return RedirectToAction("ManageEvents");
        }

        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateEvent(
    CreateEventViewModel model)
{
    if (HttpContext.Session.GetString("Role") != "Organiser")
    {
        return RedirectToAction("Index", "Home");
    }

    if (!ModelState.IsValid)
    {
        return View(model);
    }

    var token = HttpContext.Session.GetString("Token");

    var success = await _eventApiService.CreateEventAsync(
        model, token!);

    if (!success)
    {
        ModelState.AddModelError(
            string.Empty,
            "Unable to create event. Please try again.");

        return View(model);
    }

    TempData["Success"] = "Event created successfully!";

    return RedirectToAction("Dashboard");
}
    }
}