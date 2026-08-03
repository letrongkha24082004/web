using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BlogManager_LeTrongKha.Models;

namespace BlogManager_LeTrongKha.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        ViewData["Title"] = "Giới thiệu";
        return View();
    }

    public IActionResult Contact()
    {
        ViewData["Title"] = "Liên hệ";
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
