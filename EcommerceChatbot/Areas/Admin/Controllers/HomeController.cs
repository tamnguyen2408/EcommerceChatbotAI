using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceChatbot.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin")]

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
