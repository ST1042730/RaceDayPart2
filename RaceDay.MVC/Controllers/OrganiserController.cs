
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using RaceDay.MVC.Models;

namespace RaceDay.MVC.Controllers;

public class OrganiserController : Controller
{
    private readonly IHttpClientFactory _factory;

    public OrganiserController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient GetApi()
    {
        var api = _factory.CreateClient("RaceDayAPI");
        var token = HttpContext.Session.GetString("Token");

        if (!string.IsNullOrWhiteSpace(token))
        {
            api.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return api;
    }

    private bool IsOrganiser()
    {
        return HttpContext.Session.GetString("Role") == "Organiser";
    }

    public IActionResult Index()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    // EVENT MANAGEMENT

    public async Task<IActionResult> Events()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var events = await GetApi()
                .GetFromJsonAsync<List<EventViewModel>>("api/events");

            return View(events ?? new List<EventViewModel>());
        }
        catch (Exception ex)
        {
            Console.WriteLine("EVENTS ERROR: " + ex.Message);
            ViewBag.Error = "Unable to load events.";
            return View(new List<EventViewModel>());
        }
    }

    [HttpGet]
    public IActionResult CreateEvent()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(CreateEventViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await GetApi()
                .PostAsJsonAsync("api/events", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction("Events");
            }

            ModelState.AddModelError(
                "",
                "API error: " + await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            Console.WriteLine("CREATE EVENT ERROR: " + ex.Message);
            ModelState.AddModelError("", "Unable to connect to the API.");
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditEvent(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var race = await GetApi()
                .GetFromJsonAsync<EventViewModel>($"api/events/{id}");

            if (race == null)
                return NotFound();

            var model = new CreateEventViewModel
            {
                EventId = race.EventId,
                EventName = race.EventName,
                Description = race.Description,
                EventDate = race.EventDate,
                Location = race.Location,
                DistanceKm = race.DistanceKm,
                EventType = race.EventType,
                Status = race.Status
            };

            return View(model);
        }
        catch (Exception ex)
        {
            Console.WriteLine("EDIT EVENT ERROR: " + ex.Message);
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEvent(CreateEventViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await GetApi().PutAsJsonAsync(
                $"api/events/{model.EventId}", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction("Events");
            }

            ModelState.AddModelError(
                "",
                "API error: " + await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            Console.WriteLine("UPDATE EVENT ERROR: " + ex.Message);
            ModelState.AddModelError("", "Unable to connect to the API.");
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var response = await GetApi().DeleteAsync($"api/events/{id}");

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Event deleted successfully!";
            else
                TempData["Error"] =
                    await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("DELETE EVENT ERROR: " + ex.Message);
            TempData["Error"] = "Unable to connect to the API.";
        }

        return RedirectToAction("Events");
    }

    // CATEGORY MANAGEMENT

    public async Task<IActionResult> Categories()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var categories = await GetApi()
                .GetFromJsonAsync<List<CategoryViewModel>>("api/categories");

            return View(categories ?? new List<CategoryViewModel>());
        }
        catch (Exception ex)
        {
            Console.WriteLine("CATEGORIES ERROR: " + ex.Message);
            ViewBag.Error = "Unable to load categories.";
            return View(new List<CategoryViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CategoryViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (string.IsNullOrWhiteSpace(model.CategoryName))
        {
            TempData["Error"] = "Category name is required.";
            return RedirectToAction("Categories");
        }

        try
        {
            var response = await GetApi()
                .PostAsJsonAsync("api/categories", model);

            if (response.IsSuccessStatusCode)
                TempData["Success"] = "Category created successfully!";
            else
                TempData["Error"] =
                    await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("CREATE CATEGORY ERROR: " + ex.Message);
            TempData["Error"] = "Unable to connect to the API.";
        }

        return RedirectToAction("Categories");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignCategory(
        AssignCategoryViewModel model)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter valid category details.";
            return RedirectToAction("Categories");
        }

        try
        {
            var response = await GetApi().PostAsJsonAsync(
                "api/categories/event", model);

            if (response.IsSuccessStatusCode)
                TempData["Success"] =
                    "Category assigned to event successfully!";
            else
                TempData["Error"] =
                    await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("ASSIGN CATEGORY ERROR: " + ex.Message);
            TempData["Error"] = "Unable to connect to the API.";
        }

        return RedirectToAction("Categories");
    }

    // ORGANISER ENROLMENTS

    public async Task<IActionResult> Enrolments()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var events = await GetApi()
                .GetFromJsonAsync<List<EventViewModel>>("api/events");

            return View(events ?? new List<EventViewModel>());
        }
        catch (Exception ex)
        {
            Console.WriteLine("ENROLMENTS ERROR: " + ex.Message);
            ViewBag.Error = "Unable to load events.";
            return View(new List<EventViewModel>());
        }
    }

    public async Task<IActionResult> EventEnrolments(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        try
        {
            var api = GetApi();

            var race = await api.GetFromJsonAsync<EventViewModel>(
                $"api/events/{id}");

            if (race == null)
                return NotFound();

            var response = await api.GetAsync(
                $"api/results/event/{id}");

            var debugJson = await response.Content.ReadAsStringAsync();

            Console.WriteLine("====================================");
            Console.WriteLine("EVENT RESULTS HTTP STATUS: " +
                (int)response.StatusCode);
            Console.WriteLine("EVENT RESULTS API RESPONSE:");
            Console.WriteLine(debugJson);
            Console.WriteLine("====================================");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error =
                    "Could not load event results: " + debugJson;

                return View(new List<OrganiserEnrolmentViewModel>());
            }

            var enrolments = JsonSerializer.Deserialize<
                List<OrganiserEnrolmentViewModel>>(
                    debugJson,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            ViewBag.EventName = race.EventName;
            ViewBag.EventId = id;

            return View(enrolments ??
                new List<OrganiserEnrolmentViewModel>());
        }
        catch (Exception ex)
        {
            Console.WriteLine("EVENT ENROLMENTS ERROR: " + ex);

            ViewBag.Error = "Error: " + ex.Message;

            return View(new List<OrganiserEnrolmentViewModel>());
        }
    }

    // RECORD RACE RESULTS

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordResult(
        RecordResultViewModel model, int eventId)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "Please enter a valid finishing time and position.";

            return RedirectToAction(
                "EventEnrolments", new { id = eventId });
        }

        try
        {
            var resultRequest = new
            {
                model.EnrolmentId,
                FinishTime = model.FinishTime.ToString(@"hh\:mm\:ss"),
                model.Position,
                model.ResultStatus
            };

            var response = await GetApi().PostAsJsonAsync(
                "api/results", resultRequest);

            if (response.IsSuccessStatusCode)
                TempData["Success"] =
                    "Race result recorded successfully!";
            else
                TempData["Error"] =
                    "API error: " +
                    await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("RECORD RESULT ERROR: " + ex);
            TempData["Error"] = "Error: " + ex.Message;
        }

        return RedirectToAction(
            "EventEnrolments", new { id = eventId });
    }
}
