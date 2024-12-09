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

            // Gọi LoginAsync từ AuthService
            var result = await _authService.LoginAsync(model.UserName, model.Password, model.Role);

            if (result != "Login successful!")
            {
                ModelState.AddModelError("", result);
                return View(model);
            }

            // Chuyển hướng dựa trên role
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
            // Lấy thông tin về vai trò người dùng hiện tại từ ClaimsPrincipal
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            // Đăng xuất dựa trên vai trò người dùng
            if (role == "admin")
            {
                await HttpContext.SignOutAsync("AdminCookie");
            }
            else
            {
                await HttpContext.SignOutAsync("UserCookie");
            }

            TempData["message"] = "Successfully logged out."; // Thông báo đăng xuất thành công

            // Chuyển hướng đến trang login chung
            return RedirectToAction("Index", "Home");
        }

    }
}