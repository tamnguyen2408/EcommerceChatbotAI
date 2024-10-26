using EcommerceChatbot.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EcommerceChatbot.Controllers
{
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost("auth/logout")]

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie"); // Adjust the authentication scheme if necessary
            return RedirectToAction("Index", "Home", new { area = "" }); // Redirect to the admin login page
        }
    }
}
