using Microsoft.AspNetCore.Mvc;

namespace EcommerceChatbot.Controllers
{
    public class OrderConfirmationController : Controller
    {
        public IActionResult OrderConfirmations()
        {
            return View();
        }
    }
}
