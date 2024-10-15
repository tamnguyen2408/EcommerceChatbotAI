using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceChatbot.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Route("admin/HomeAdmin")]

    public class HomeController : Controller
    {
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthCookie"); // Adjust the authentication scheme if necessary
            return RedirectToAction("Index", "Login", new { area = "Admin" }); // Redirect to the admin login page
        }
    }
}
