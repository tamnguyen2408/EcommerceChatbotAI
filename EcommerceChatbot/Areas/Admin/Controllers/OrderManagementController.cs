using Microsoft.AspNetCore.Mvc;

namespace EcommerceChatbot.Areas.Admin.Controllers
{
    public class OrderManagementController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
