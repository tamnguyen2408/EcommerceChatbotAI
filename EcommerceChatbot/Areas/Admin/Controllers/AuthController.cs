using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using EcommerceChatbot.Service;
using System.Threading.Tasks;
using ECommerceChatbot.Models;

namespace EcommerceChatbot.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: Admin/Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model.UserName, model.Password, model.Role);

            if (result != "Login successful!")
            {
                ModelState.AddModelError("", result);
                return View(model);
            }

            // Redirect based on role
            if (model.Role == "admin")
            {
                return RedirectToAction("Index", "Home", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }

        // POST: Admin/Auth/Logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.SignOutAsync(); // Call SignOutAsync

            // Redirect to login view of the admin area
            return RedirectToAction("Login", "Auth", new { area = "Admin" });
        }
    }
}
