using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceChatbot.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]
    [Authorize(Roles = "admin")] // Only allow admin users

    public class HomeController : Controller
    {
        [Route("")]
        [Route("index")]
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
