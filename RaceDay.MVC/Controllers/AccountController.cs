
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.MVC.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _factory;

    public AccountController(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient API =>
        _factory.CreateClient("RaceDayAPI");

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
            return View(model);

        try
        {
            var response = await API.PostAsJsonAsync(
                "api/auth/login",
                model
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", "Login failed: " + error);
                return View(model);
            }

            using var json = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync()
            );

            var token = json.RootElement
                .GetProperty("token").GetString();

            var role = json.RootElement
                .GetProperty("role").GetString();

            if (string.IsNullOrWhiteSpace(token) ||
                (role != "Organiser" && role != "Participant"))
            {
                ModelState.AddModelError("", "Invalid login response.");
                return View(model);
            }

            HttpContext.Session.SetString("Token", token);
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("Email", model.Email);

            if (role == "Organiser")
                return RedirectToAction("Index", "Organiser");

            return RedirectToAction("Index", "Participant");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                "",
                "Connection error: " + ex.Message
            );

            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await API.PostAsJsonAsync(
                "api/auth/register",
                model
            );

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();

                ModelState.AddModelError(
                    "",
                    "API ERROR: " + error
                );

                return View(model);
            }

            TempData["Success"] =
                "Registration successful. You can now log in.";

            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(
                "",
                "Connection error: " + ex.Message
            );

            return View(model);
        }
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
