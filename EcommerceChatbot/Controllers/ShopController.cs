using Microsoft.AspNetCore.Mvc;

namespace EcommerceChatbot.Controllers
{
    public class ShopController : Controller
    {
        [HttpGet("Shop/Shoes")]
        public IActionResult Shoes()
        {
            return View(Shoes);  // Ensure this matches the view name.
        }


        public IActionResult Clothes()
        {
            // Logic xử lý cho trang Clothes Shop
            return View();
        }

        public IActionResult Handbags()
        {
            // Logic xử lý cho trang Handbags Shop
            return View();
        }
        public IActionResult Index()
        {
            // Logic xử lý cho trang chính của Shop
            return View();
        }

    }
}
