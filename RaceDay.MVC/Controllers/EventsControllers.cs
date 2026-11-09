
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using RaceDay.MVC.Models;

namespace RaceDay.MVC.Controllers;

public class EventsController : Controller
{
    private readonly IHttpClientFactory _factory;

    public EventsController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<IActionResult> Details(int id)
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return RedirectToAction("Login", "Account");

        try
        {
            var api = _factory.CreateClient("RaceDayAPI");

            var race = await api.GetFromJsonAsync<EventViewModel>(
                $"api/events/{id}"
            );

            if (race == null)
                return NotFound();

            var categories = await api.GetFromJsonAsync<List<EventCategoryViewModel>>(
                $"api/categories/event/{id}"
            );

            var model = new RaceDetailsViewModel
            {
                Event = race,
                Categories = categories ?? new List<EventCategoryViewModel>()
            };

            return View(model);
        }
        catch
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enrol(int eventCategoryId, int eventId)
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return RedirectToAction("Login", "Account");

        var token = HttpContext.Session.GetString("Token");

        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        try
        {
            var api = _factory.CreateClient("RaceDayAPI");

            api.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await api.PostAsJsonAsync(
                "api/enrolments",
                new { EventCategoryId = eventCategoryId }
            );

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] =
                    "You have successfully enrolled in this race!";
            }
            else
            {
                TempData["Error"] =
                    await response.Content.ReadAsStringAsync();
            }
        }
        catch
        {
            TempData["Error"] = "Unable to connect to the RaceDay API.";
        }

        return RedirectToAction("Details", new { id = eventId });
    }
}

