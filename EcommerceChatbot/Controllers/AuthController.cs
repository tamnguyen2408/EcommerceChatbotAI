using Microsoft.AspNetCore.Mvc;
using ECommerceChatbot.Models;
using System.Threading.Tasks;
using EcommerceChatbot.Models;
using Microsoft.EntityFrameworkCore;
using EcommerceChatbot.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace ECommerceChatbot.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService) // Thay đổi ECommerceAiDbContext thành AuthService
        {
            _authService = authService;
        }

        // GET: auth/register
        [HttpGet("/auth/register")]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost("auth/register")]
        public async Task<IActionResult> Register(Register model)
        {
            if (ModelState.IsValid)
            {
                var result = await _authService.RegisterAsync(model.UserName, model.Email, model.Password, model.ConfirmPassword, model.Phone);
                if (result == "User registered successfully!")
                {
                    return RedirectToAction("Login");
                }
                ModelState.AddModelError("", result);
            }
            return View(model);
        }
        [HttpGet("auth/login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated) // Kiểm tra xem người dùng đã đăng nhập chưa
            {
                return RedirectToAction("Index", "Home");
            }

            return View(); // Trả về view đăng nhập nếu chưa đăng nhập
        }


        [HttpPost("/auth/login")]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Call LoginAsync from AuthService
            var result = await _authService.LoginAsync(model.UserName, model.Password, model.Role);

            if (result != "Login successful!")
            {
                ModelState.AddModelError("", result);
                return View(model);
            }

            // Create claims for the user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.Role, model.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe, // Set persistent based on the checkbox
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : null // Optional: Set expiration
            };

            await HttpContext.SignInAsync("AuthCookie", new ClaimsPrincipal(claimsIdentity), authProperties);

            // Redirect based on role
            if (model.Role == "admin")
            {
                return RedirectToAction("Index", "Home", new { area = "admin" });
            }
            else
            {
                return RedirectToAction("Index", "User");
            }
        }

        [HttpPost("auth/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie"); // Đăng xuất người dùng
            TempData["message"] = "Successfully logged out."; // Thông báo đăng xuất thành công

            // Chuyển hướng đến trang login chung
            return RedirectToAction("Login", "Auth");
        }

    }
}