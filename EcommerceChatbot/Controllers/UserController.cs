using Microsoft.AspNetCore.Mvc;

namespace EcommerceChatbot.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
