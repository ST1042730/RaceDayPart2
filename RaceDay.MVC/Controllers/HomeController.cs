
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (HttpContext.Session.GetString("Role") == "Organiser")
            return RedirectToAction("Index", "Organiser");

        if (HttpContext.Session.GetString("Role") == "Participant")
            return RedirectToAction("Index", "Participant");

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}
