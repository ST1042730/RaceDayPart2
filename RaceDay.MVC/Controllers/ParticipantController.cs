using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using RaceDay.MVC.Models;

namespace RaceDay.MVC.Controllers;

public class ParticipantController : Controller
{
    private readonly IHttpClientFactory _factory;

    public ParticipantController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return RedirectToAction("Login", "Account");

        try
        {
            var api = _factory.CreateClient("RaceDayAPI");

            var events = await api.GetFromJsonAsync<List<EventViewModel>>(
                "api/events"
            );

            var upcomingEvents = events?
                .Where(e => e.EventDate >= DateTime.Today &&
                            e.Status == "Upcoming")
                .OrderBy(e => e.EventDate)
                .ToList() ?? new List<EventViewModel>();

            return View(upcomingEvents);
        }
        catch
        {
            ViewBag.Error = "Unable to load races. Please try again later.";
            return View(new List<EventViewModel>());
        }
    }

    public async Task<IActionResult> MyEvents()
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

            var enrolments =
                await api.GetFromJsonAsync<List<ParticipantEnrolmentViewModel>>(
                    "api/enrolments/mine"
                );

            return View(enrolments ?? new List<ParticipantEnrolmentViewModel>());
        }
        catch
        {
            ViewBag.Error = "Unable to load your enrolled races.";
            return View(new List<ParticipantEnrolmentViewModel>());
        }
    }

    public async Task<IActionResult> MyResults()
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

            var results =
                await api.GetFromJsonAsync<List<ParticipantResultViewModel>>(
                    "api/results/mine"
                );

            return View(results ?? new List<ParticipantResultViewModel>());
        }
        catch
        {
            ViewBag.Error = "Unable to load your results.";
            return View(new List<ParticipantResultViewModel>());
        }
    }

    // VIEW PARTICIPANT PROFILE
    [HttpGet]
    public async Task<IActionResult> Profile()
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

            var profile = await api.GetFromJsonAsync<ProfileViewModel>(
                "api/profile"
            );

            if (profile == null)
                return RedirectToAction("Index");

            return View(profile);
        }
        catch
        {
            TempData["Error"] = "Unable to load your profile.";
            return RedirectToAction("Index");
        }
    }

    // UPDATE PARTICIPANT PROFILE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (HttpContext.Session.GetString("Role") != "Participant")
            return RedirectToAction("Login", "Account");

        var token = HttpContext.Session.GetString("Token");

        if (string.IsNullOrWhiteSpace(token))
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var api = _factory.CreateClient("RaceDayAPI");

            api.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await api.PutAsJsonAsync("api/profile", new
            {
                model.FirstName,
                model.LastName
            });

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError(
                "",
                "Unable to update your profile. Please try again."
            );

            return View(model);
        }
        catch
        {
            ModelState.AddModelError(
                "",
                "Could not connect to the API."
            );

            return View(model);
        }
    }
}